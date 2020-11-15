using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO
{
    public class AnalyseKlassen
    {
        public List<string> dataTypes = new List<string> { "void", "bool", "byte", "char", "decimal", "double", "enum", "float", "int", "long", "sbyte", "short", "string", "struct", "uint", "ulong", "ushort" };
        public List<string> classDataTypes = new List<string>();
        public char[] chars = new char[] { ' ', ':', ';', '{', '=' };
        public Input Input { get; set; } = new Input();
        public OutputClass Output { get; set; } = new OutputClass();
        public void IterateFoldersAndFiles()
        {
            foreach (string f in Input.UitgepaktZipFolders)
            {
                Output.CreateNewDirectory(Path.GetFileNameWithoutExtension(f));
                IterateFilesAndAnalyse(Path.GetFullPath(f));
            }
        }
        public void IterateFilesAndAnalyse(string folderPath)
        {
            ClassAsDataTypeAdder(folderPath);
            foreach (string f in Directory.GetFiles(folderPath))
            {
                if (f.Contains(".cs"))
                {
                    string code = File.ReadAllText(f).Replace('\r', ' ').Replace('\n', ' ');
                    ClassInfo output = CheckStringsAndAnalyse(code);
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output);
                }
            }
        }
        public ClassInfo CheckStringsAndAnalyse(string s)
        {
            ClassInfo output = new ClassInfo();
            if (UsingAdder(s) != null) output.Usings.Add(UsingAdder(s));
            if (NamespaceAdder(s) != null) output.Namespace = NamespaceAdder(s);
            if (ClassAdder(s) != null) output.Name = ClassAdder(s);
            if (InheritAdder(s) != null) output.Inherits.Add(InheritAdder(s));
            if (ConstructorAdder(s, output) != null) output.Constructors.Add(ConstructorAdder(s, output));
            if (MethodAdder(s) != null) output.Methods.Add(MethodAdder(s));
            if (PropertyAdder(s) != null) output.Properties.Add(PropertyAdder(s));
            if (VariableAdder(s) != null) output.Variables.Add(VariableAdder(s));
            return output;
        }
        public string UsingAdder(string s)
        {
            if (s.Contains("using "))
                return s.Substring(s.IndexOf("using") + 6).Trim().Split(chars, StringSplitOptions.None)[0];
            return null;
        }
        public string NamespaceAdder(string s)
        {
            if (s.Contains("namespace "))
                return s.Substring(s.IndexOf("namespace") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            return null;
        }
        public string ClassAdder(string s)
        {
            if (s.Contains(" class "))
            {
                return s.Substring(s.IndexOf("class") + 6).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            else if (s.Contains(" interface "))
            {
                return s.Substring(s.IndexOf("interface") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            return null;
        }
        public string InheritAdder(string s)
        {
            if (s.Contains(" class ") || s.Contains(" interface "))
            {
                if (s.Contains(":"))
                    return s.Substring(s.IndexOf(":") + 2).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            return null;
        }
        public string MethodAdder(string s)
        {
            if (s.Contains("(") && s.Contains(")"))
            {
                foreach (string d in dataTypes)
                {
                    if (s.Contains(d))
                        return s.Substring(s.IndexOf(d) + d.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                }
                foreach (string c in classDataTypes)
                {
                    var methodNaam = s.Substring(s.IndexOf(c) + c.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                    if (s.Contains(c) && methodNaam != c)
                        return methodNaam;
                }
            }
            return null;
        }
        public string ConstructorAdder(string s, ClassInfo output)
        {
            if (s.Contains("(") && s.Contains(")"))
            {
                if (s.Contains(output.Name))
                {
                    var constructorNaam = s.Substring(s.IndexOf(output.Name)).Trim().Split(chars, StringSplitOptions.None)[0];
                    if (constructorNaam == output.Name)
                        return constructorNaam;
                }
            }
            return null;
        }
        public string PropertyAdder(string s)
        {
            if (s.Contains("get") || s.Contains("set"))
            {
                foreach (string d in dataTypes)
                {
                    if (s.Contains(d))
                        return s.Substring(s.IndexOf(d) + d.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                }
                foreach (string c in classDataTypes)
                {
                    if (s.Contains(c))
                        return s.Substring(s.IndexOf(c) + c.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                }
            }
            return null;
        }
        public string VariableAdder(string s)
        {
            foreach (string d in dataTypes)
            {
                if (s.Contains(d) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(d) + d.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            foreach (string c in classDataTypes)
            {
                if (s.Contains(c) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(c) + c.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            return null;
        }
        public void ClassAsDataTypeAdder(string folderPath)
        {
            foreach (string f in Directory.GetFiles(folderPath))
            {
                if (f.Contains(".cs"))
                {
                    string code = File.ReadAllText(f).Replace('\r', ' ').Replace('\n', ' ');
                    if (code.Contains(" class ") && !code.Contains(" abstract "))
                    {
                        classDataTypes.Add(code.Substring(code.IndexOf("class") + 6).Trim().Split(chars, StringSplitOptions.None)[0]);
                    }
                }
            }
        }
    }
}
