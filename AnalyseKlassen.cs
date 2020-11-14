using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileIO
{
    public class AnalyseKlassen
    {
        public List<string> dataTypes = new List<string> { "void", "bool", "byte", "char", "decimal", "double", "enum", "float", "int", "long", "sbyte", "short", "string", "struct", "uint", "ulong", "ushort" };
        public List<string> classDataTypes = new List<string>();
        public Input Input { get; set; }
        public Output Output { get; set; }
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
                    List<ClassInfo> output = new List<ClassInfo>();
                    CheckStringsAndAnalyse(File.ReadAllLines(f), output);
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output, "ClassInfo");
                }
            }
        }
        public void CheckStringsAndAnalyse(string[] allLines, List<ClassInfo> outputList)
        {
            ClassInfo output = new ClassInfo();
            foreach (string s in allLines)
            {
                if (UsingAdder(s) != null) output.Usings.Add(UsingAdder(s));
                if (NamespaceAdder(s) != null) output.Namespace = NamespaceAdder(s);
                if (ClassAdder(s) != null) output.Name = ClassAdder(s);
                if (InheritAdder(s) != null) output.Inherits.Add(InheritAdder(s));
                if (ConstructorAdder(s, output) != null) output.Constructors.Add(ConstructorAdder(s, output));
                if (MethodAdder(s) != null) output.Methods.Add(MethodAdder(s));
                if (PropertyAdder(s) != null) output.Properties.Add(PropertyAdder(s));
                if (VariableAdder(s) != null) output.Variables.Add(VariableAdder(s));
            }
            outputList.Add(output);
        }
        public string UsingAdder(string s)
        {
            if (s.Contains("using "))
                return s.Substring(s.IndexOf("using") + 6, Math.Abs(s.IndexOf("using") + 5 - s.IndexOf(";")) - 1);
            return null;
        }
        public string NamespaceAdder(string s)
        {
            if (s.Contains("namespace "))
                return s.Substring(s.IndexOf("namespace") + 10);
            return null;
        }
        public string ClassAdder(string s)
        {
            if (s.Contains("class "))
            {
                if (s.Contains(":"))
                    return s.Substring(s.IndexOf("class") + 6, Math.Abs(s.IndexOf("class") + 5 - s.IndexOf(":")) - 1);
                else
                    return s.Substring(s.IndexOf("class") + 6);
            }
            else if (s.Contains("interface "))
            {
                if (s.Contains(":"))
                    return s.Substring(s.IndexOf("interface") + 10, Math.Abs(s.IndexOf("interface") + 9 - s.IndexOf(":")) - 1);
                else
                    return s.Substring(s.IndexOf("interface") + 10);
            }
            return null;
        }
        public string InheritAdder(string s)
        {
            if (s.Contains("class ") || s.Contains("interface "))
            {
                if (s.Contains(":"))
                    return s.Substring(s.IndexOf(":") + 2);
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
                        return s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("(")) - 1);
                }
                foreach (string c in classDataTypes)
                {
                    var methodNaam = s.Substring(s.IndexOf(c) + c.Length + 1, Math.Abs(s.IndexOf(c) + c.Length - s.IndexOf("(")) - 1);
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
                    var constructorNaam = s.Substring(s.IndexOf(output.Name) + output.Name.Length + 1, Math.Abs(s.IndexOf(output.Name) + output.Name.Length - s.IndexOf("(")) - 1);
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
                        return s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("{")) - 2);
                }
                foreach (string c in classDataTypes)
                {
                    if (s.Contains(c))
                        return s.Substring(s.IndexOf(c) + c.Length + 1, Math.Abs(s.IndexOf(c) + c.Length - s.IndexOf("{")) - 2);
                }
            }
            return null;
        }
        public string VariableAdder(string s)
        {
            foreach (string d in dataTypes)
            {
                if ((s.Contains(d) && s.Contains("=")) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("=")) - 2);
                else if ((s.Contains(d) && s.Contains(";")) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf(";")) - 1);
            }
            foreach (string c in classDataTypes)
            {
                if ((s.Contains(c) && s.Contains("=")) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(c) + c.Length + 1, Math.Abs(s.IndexOf(c) + c.Length - s.IndexOf("=")) - 2);
                else if ((s.Contains(c) && s.Contains(";")) && (!s.Contains("get") || !s.Contains("set")))
                    return s.Substring(s.IndexOf(c) + c.Length + 1, Math.Abs(s.IndexOf(c) + c.Length - s.IndexOf(";")) - 1);
            }
            return null;
        }
        public void ClassAsDataTypeAdder(string folderPath)
        {
            foreach (string f in Directory.GetFiles(folderPath))
            {
                if (f.Contains(".cs"))
                {
                    foreach (string s in File.ReadLines(f))
                    {
                        if (s.Contains("class ") && !s.Contains("abstract "))
                        {
                            if (s.Contains(":"))
                                classDataTypes.Add(s.Substring(s.IndexOf("class") + 6, Math.Abs(s.IndexOf("class") + 6 - s.IndexOf(":")) - 1));
                            else
                                classDataTypes.Add(s.Substring(s.IndexOf("class") + 6));
                        }
                    }
                }
            }

        }
    }
}
