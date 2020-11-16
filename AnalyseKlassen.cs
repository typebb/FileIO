using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO
{
    public class AnalyseKlassen
    {
        public List<string> dataTypes = new List<string> { "bool", "byte", "char", "decimal", "double", "enum", "float", "int", "long", "sbyte", "short", "string", "struct", "uint", "ulong", "ushort" };
        public List<string> methodReturnTypes = new List<string> { "void", "bool", "byte", "char", "decimal", "double", "enum", "float", "int", "long", "sbyte", "short", "string", "struct", "uint", "ulong", "ushort" };

        public List<string> classDataTypes = new List<string>();
        public char[] chars = new char[] { ' ', ':', ';', '{', '}', '=' };
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
                    ClassInfo output = new ClassInfo();
                    UsingAdder(code, output);
                    ReadUntilNextMatchingBrace(code, output);
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output);
                }
            }
        }
        public void ReadUntilNextMatchingBrace(string code, ClassInfo output)
        {
            int nOpen = 0;
            int indexOpen;
            int indexClose;
            do
            {
                indexOpen = code.IndexOf("{");
                if (indexOpen == -1) indexOpen = code.Length;
                indexClose = code.IndexOf("}");
                if (indexOpen < indexClose)
                {
                    nOpen++;
                    code = code.Substring(indexOpen + 1);
                }
                if (indexOpen > indexClose)
                {
                    nOpen--;
                    code = code.Substring(indexClose + 1);
                }
                CheckStringsAndAnalyse(code, output);
            }
            while (nOpen > 0);
        }
        public void CheckStringsAndAnalyse(string s, ClassInfo output)
        {
            ClassAdder(s, output);
            InheritAdder(s, output);
            ConstructorAdder(s, output);
            MethodAdder(s, output);
            PropertyAdder(s, output);
            VariableAdder(s, output);
        }
        public void UsingAdder(string s, ClassInfo output)
        {
            NamespaceAdder(s, output);
            if (s.Contains("using "))
            {
                var code = s.Trim().Split(new char[] { '{' }, StringSplitOptions.None);
                code = code[0].Split(new char[] { ';'}, StringSplitOptions.None);
                foreach(string u in code)
                {
                    if (!u.Contains("namespace"))
                        output.Usings.Add(u.Substring(s.IndexOf("using") + 6).Trim());
                }
            }
        }
        public void NamespaceAdder(string s, ClassInfo output)
        {
            if (s.Contains("namespace "))
                output.Namespace = s.Substring(s.IndexOf("namespace") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
        }
        public void ClassAdder(string s, ClassInfo output)
        {
            if (s.Contains(" class "))
            {
                output.Name = s.Substring(s.IndexOf("class") + 6).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            else if (s.Contains(" interface "))
            {
                output.Name = s.Substring(s.IndexOf("interface") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            }
        }
        public void InheritAdder(string s, ClassInfo output)
        {
            if (s.Contains(" class ") || s.Contains(" interface "))
            {
                if (s.Contains(":"))
                {
                    var code = s.Substring(s.IndexOf(":") + 2).Trim().Split(chars, StringSplitOptions.None);
                    code = code[0].Split(new char[] { ',' }, StringSplitOptions.None);
                    foreach (string i in code)
                        output.Inherits.Add(i);
                }
            }
        }
        public void MethodAdder(string s, ClassInfo output)
        {
            if (s.Contains("(") && s.Contains(")"))
            {
                foreach (string d in dataTypes)
                {
                    var methodNaam = s.Substring(s.IndexOf(d) + d.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                    if (s.Contains(d) && methodNaam.Contains("("))
                        output.Methods.Add(methodNaam);
                }
                // check of lukt!!!!!!!!!!!!!!!!!!!!
                foreach (string d in methodReturnTypes)
                {
                    var methodNaam = s.Substring(s.IndexOf(d) + d.Length + 1).Trim().Split(chars, StringSplitOptions.None)[0];
                    if (s.Contains(d) && methodNaam.Contains("(")) // (!c.Contains($"{c}(") || !c.Contains($"{c} ("))
                        output.Methods.Add(methodNaam);
                }
            }
        }
        public void ConstructorAdder(string s, ClassInfo output)
        {
            if (s.Contains("(") && s.Contains(")"))
            {
                if (s.Contains($" {output.Name}(") || s.Contains($" {output.Name} ("))
                {
                    var constructorNaam = s.Substring(s.IndexOf(output.Name)).Trim().Split(new char[] { ';', '=', '{', '}' }, StringSplitOptions.None);
                    foreach (string f in constructorNaam)
                    {
                        if (f.Contains(")") && (f.Contains($" {output.Name}(") || f.Contains($" {output.Name} (")))
                            output.Constructors.Add(f.Substring(f.IndexOf(output.Name)));
                    }
                }
            }
        }
        public void PropertyAdder(string s, ClassInfo output)
        {
            if (s.Contains("get") || s.Contains("set"))
            {
                foreach (string d in dataTypes)
                {
                    if (s.Contains(d))
                    {
                        var code = s.Trim().Split(new char[] { ':', ';', '=', '}', ')' }, StringSplitOptions.None);
                        foreach (string c in code)
                        {
                            if (c.Contains($" {d} ") && !c.Contains($"( {d}") && (!c.Contains($",{d}") || !c.Contains($", {d}")) && (c.Contains("get") || c.Contains("set")))
                                output.Properties.Add(c.Substring(c.IndexOf(d) + d.Length + 1).Trim().Split(new char[] { ':', ';', '=', '{', '}', ')' }, StringSplitOptions.None)[0]);
                        }
                    }
                }
                foreach (string d in classDataTypes)
                {
                    if (s.Contains(d))
                    {
                        var code = s.Trim().Split(new char[] { ':', ';', '=', '}', ')' }, StringSplitOptions.None);
                        foreach (string c in code)
                        {
                            if (c.Contains($" {d} ") && !c.Contains($"( {d}") && (!c.Contains($",{d}") || !c.Contains($", {d}")) && (c.Contains("get") || c.Contains("set")))
                                output.Properties.Add(c.Substring(c.IndexOf(d) + d.Length + 1).Trim().Split(new char[] { ':', ';', '=', '{', '}', ')' }, StringSplitOptions.None)[0]);
                        }
                    }
                }
            }
        }
        public void VariableAdder(string s, ClassInfo output)
        {
            foreach (string d in dataTypes)
            {
                if (s.Contains(d))
                {
                    var code = s.Trim().Split(new char[] { ':', ';', '}' }, StringSplitOptions.None);
                    foreach(string c in code)
                    {
                        if (c.Contains($" {d} ") && !c.Contains(">") && (c.Substring(c.IndexOf(d) + d.Length) != "=") && !c.Contains("{") && !c.Contains(")") && (!c.Contains($"({d}") || !c.Contains($"( {d}")) && (!c.Contains($",{d}") || !c.Contains($", {d}")) && (!c.Contains("get") || !c.Contains("set")))
                            output.Variables.Add(c.Substring(c.IndexOf(d) + d.Length + 1).Trim().Split(new char[] { ':', ';', '=', '}', ')' }, StringSplitOptions.None)[0]);
                    }
                }
            }
            foreach (string d in classDataTypes)
            {
                if (s.Contains(d))
                {
                    var code = s.Trim().Split(new char[] { ':', ';', '}' }, StringSplitOptions.None);
                    foreach (string c in code)
                    {
                        if (c.Contains($" {d} ") && !c.Contains(">") && (c.Substring(c.IndexOf(d) + d.Length) != "=") && !c.Contains("{") && !c.Contains(")") && (!c.Contains($"({d}") || !c.Contains($"( {d}")) && (!c.Contains($",{d}") || !c.Contains($", {d}")) && (!c.Contains("get") || !c.Contains("set")))
                            output.Variables.Add(c.Substring(c.IndexOf(d) + d.Length).Trim().Split(new char[] { ':', ';', '=', '}', ')' }, StringSplitOptions.None)[0]);
                    }
                }
            }
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
                        classDataTypes.Add(code.Substring(code.IndexOf(" class ") + 6).Trim().Split(chars, StringSplitOptions.None)[0]);
                    }
                }
            }
        }
    }
}
