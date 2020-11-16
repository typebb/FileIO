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
                    BestandInfo output = new BestandInfo();
                    NamespaceAdder(code, output);
                    ReadUntilNextMatchingBrace(code, output);
                    if (output.Name != null && output.Namespace != null)
                    {
                        CodeTeller(f, output);
                        Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output);
                    }
                }
            }
        }
        public void ReadUntilNextMatchingBrace(string code, BestandInfo output)
        {
            int tellerKlassen = 0;
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
                CheckStringsAndAnalyse(code, output, ref tellerKlassen);
            }
            while (nOpen > 0);
        }
        public void CheckStringsAndAnalyse(string s, BestandInfo output, ref int tellerKlassen)
        {
            KlassenTeller(s, ref tellerKlassen);
            ClassAdder(s, output);
        }
        public void NamespaceAdder(string s, BestandInfo output)
        {
            if (s.Contains("namespace "))
                output.Namespace = s.Substring(s.IndexOf("namespace") + 10).Trim().Split(new char[] { '{' }, StringSplitOptions.None)[0];
        }
        public void ClassAdder(string s, BestandInfo output)
        {
            if (s.Contains(" class "))
            {
                TypeOffClass(s, output);
                output.Name = s.Substring(s.IndexOf("class") + 6).Trim().Split(chars, StringSplitOptions.None)[0];
            }
            else if (s.Contains(" interface "))
            {
                TypeOffClass(s, output);
                output.Name = s.Substring(s.IndexOf("interface") + 10).Trim().Split(chars, StringSplitOptions.None)[0];
            }
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
        public void CodeTeller(string path, BestandInfo output)
        {
            int teller = 0;
            foreach (string s in File.ReadAllLines(path))
            {
                if (s.Trim().Length > 0)
                    teller++;
            }
            output.AantalLijnenCode = teller;
        }
    }
}
