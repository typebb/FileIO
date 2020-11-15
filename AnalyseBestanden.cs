using System;
using System.IO;

namespace FileIO
{
    public class AnalyseBestanden
    {
        public char[] chars = new char[] { ' ', ':', ';', '{', '=' };
        public Input Input { get; set; } = new Input();
        public OutputBestand Output { get; set; } = new OutputBestand();
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
                    string code = File.ReadAllText(f).Replace('\r', ' ').Replace('\n', ' ');
                    BestandInfo output = CheckStringsAndAnalyse(code);
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output);
                }
            }
        }
        public BestandInfo CheckStringsAndAnalyse(string s)
        {
            BestandInfo output = new BestandInfo();
            int tellerKlassen = 0;
            int tellerLijnenCode = 0;
            KlassenTeller(s, ref tellerKlassen);
            CodeTeller(s, ref tellerLijnenCode);
            if (ClassAdder(s, output) != null) output.Name = ClassAdder(s, output);
            if (NamespaceAdder(s) != null) output.Namespace = NamespaceAdder(s);
            output.AantalLijnenCode = tellerLijnenCode;
            return output;
        }
        public string NamespaceAdder(string s)
        {
            if (s.Contains("namespace "))
                return s.Substring(s.IndexOf("namespace") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            return null;
        }
        public string ClassAdder(string s, BestandInfo output)
        {
            if (s.Contains(" class "))
            {
                TypeOffClass(s, output);
                return s.Substring(s.IndexOf("class") + 6).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            else if (s.Contains(" interface "))
            {
                TypeOffClass(s, output);
                return s.Substring(s.IndexOf("interface") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            return null;
        }
        public void TypeOffClass(string s, BestandInfo output)
        {
            if (s.Contains(" class "))
            {
                if (s.Contains("abstract "))
                    output.TypeOfClass = "abstract class";
                else
                    output.TypeOfClass = "class";
            }
            if (s.Contains(" interface "))
                output.TypeOfClass = "interface";
        }
        public void KlassenTeller(string s, ref int teller)
        {
            if (teller > 1)
                throw new Exception("Meer dan 1 klasse in de klasse file.");
            if (s.Contains(" class ") || s.Contains(" interface "))
                teller++;
        }
        public void CodeTeller(string s, ref int teller)
        {
            if (s.Trim().Length > 0)
                teller++;
        }
    }
}
