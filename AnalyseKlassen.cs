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

            foreach (string f in Directory.GetFiles(folderPath))
            {
                if (f.Contains(".cs"))
                {
                    List<string> output = new List<string>();
                    Analyseer(f, ref output);
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output, "ClassInfo");
                }
            }
        }
        public void Analyseer(string path, ref List<string> output)
        {
            string[] allLines = File.ReadAllLines(path);
            ClassAsDataTypeAdder(allLines);
            CheckStringsAndAddToList(allLines, ref output);
        }
        public void CheckStringsAndAddToList(string[] allLines, ref List<string> output)
        {
            int namespaceLocation = 0;
            foreach (string s in allLines)
            {
                UsingAdder(s, ref output);
                NamespaceAdder(s, ref output, ref namespaceLocation);
                ClassAdder(s, ref output, ref namespaceLocation);
                InheritAdder(s, ref output);
                MethodAdder(s, ref output);
                PropertyAdder(s, ref output);
                VariableAdder(s, ref output);
            }
        }
        public void UsingAdder(string s, ref List<string> output)
        {
            if (s.Contains("using"))
                output.Add($"using : {s.IndexOf("using" + 6)}");
        }
        public void NamespaceAdder(string s, ref List<string> output, ref int namespaceLocation)
        {
            if (s.Contains("namespace"))
            {
                s = s.Substring(s.IndexOf("namespace") + 10);
                output.Add(s);
                namespaceLocation = output.Count - 1;
            }
        }
        public void ClassAdder(string s, ref List<string> output, ref int namespaceLocation)
        {
            if (s.Contains("class"))
            {
                if (s.Contains(":"))
                    s = s.Substring(s.IndexOf("class") + 6, Math.Abs(s.IndexOf("class") - s.IndexOf(":")) - 1);
                else
                    s = s.Substring(s.IndexOf("class") + 6);
                output[namespaceLocation] += $", {s}";
            }
            else if (s.Contains("interface"))
            {
                if (s.Contains(":"))
                    s = s.Substring(s.IndexOf("interface") + 10, Math.Abs(s.IndexOf("class") - s.IndexOf(":")) - 1);
                else
                    s = s.Substring(s.IndexOf("interface") + 10);
                output[namespaceLocation] += $", {s}";
            }
        }
        public void InheritAdder(string s, ref List<string> output)
        {
            if (s.Contains("class") || s.Contains("interface"))
            {
                if (s.Contains(":"))
                {
                    s = $"inherit : {s.Substring(s.IndexOf(":") + 2)}";
                    output.Add(s);
                }
            }
        }
        public void MethodAdder(string s, ref List<string> output)
        {
            if (!MethodCheckWithImplementation(s, ref output))
            {
                if ((s.Contains("(") || s.Contains(")")) && !s.Contains("{"))
                    output.Add($"method : {s.Substring(s.LastIndexOf(" "))}");
                else if ((s.Contains("(") || s.Contains(")")) && s.Contains(";"))
                    output.Add($"method : {s.Substring(s.LastIndexOf(" "), Math.Abs(s.LastIndexOf(" ") - s.IndexOf(";")))}");
            }
        }
        public void PropertyAdder(string s, ref List<string> output)
        {
            if (s.Contains("get") || s.Contains("set"))
            {
                foreach (string d in dataTypes)
                {
                    if (s.Contains(d))
                        output.Add($"property : {s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("{")) - 2)}");
                }
            }
        }
        public void VariableAdder(string s, ref List<string> output)
        {
            foreach (string d in dataTypes)
            {
                if ((s.Contains(d) && s.Contains("=")) && (!s.Contains("get") || !s.Contains("set")))
                    output.Add($"variable : {s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("=")) - 2)}");
                else if ((s.Contains(d) && s.Contains(";")) && (!s.Contains("get") || !s.Contains("set")))
                    output.Add($"variable : {s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf(";")) - 1)}");
            }
        }
        public void ClassAsDataTypeAdder(string[] lines)
        {
            foreach (string s in lines)
            {
                if (s.Contains("class") && !s.Contains("abstract"))
                {
                    if (s.Contains(":"))
                        dataTypes.Add(s.Substring(s.IndexOf("class") + 6, Math.Abs(s.IndexOf("class") + 6 - s.IndexOf(":")) - 1));
                    else
                        dataTypes.Add(s.Substring(s.IndexOf("class") + 6));
                }
            }
        }
        public bool MethodCheckWithImplementation(string s, ref List<string> output)
        {
            if ((s.Contains("(") || s.Contains(")")) && s.Contains("{"))
            {
                foreach(string d in dataTypes)
                {
                    if (s.Contains(d))
                    {
                        output.Add($"method : {s.Substring(s.IndexOf(d) + d.Length + 1, Math.Abs(s.IndexOf(d) + d.Length - s.IndexOf("(")) - 1)}");
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
