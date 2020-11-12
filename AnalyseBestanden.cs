using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileIO
{
    public class AnalyseBestanden
    {
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
                    Output.WriteOutputToFile(Path.GetFileNameWithoutExtension(folderPath), output, "Analyse");
                }
            }
        }
        public void Analyseer(string path, ref List<string> output)
        {
            int tellerKlassen = 0;
            int tellerLijnenCode = 0;
            foreach (string s in File.ReadLines(path))
            {
                KlassenTeller(s, ref tellerKlassen);
                CodeTeller(s, ref tellerLijnenCode);
                CheckStringAndAddToList(s, ref output);
            }
            output.Add($"aantal lijnen code: {tellerLijnenCode}");
        }
        public void CheckStringAndAddToList(string s, ref List<string> output)
        {
            if (s.Contains("namespace") || s.Contains("interface"))
            {
                s.Insert(9, ":");
                output.Add(s);
            }
            if (s.Contains("class"))
            {
                if (s.Contains(":"))
                    s = s.Substring(s.IndexOf("class"), Math.Abs(s.IndexOf("class") - s.IndexOf(":")) - 1);
                else
                    s = s.Substring(s.IndexOf("class"));
                s.Insert(5, ":");
                output.Add(s);
            }
        }
        public void KlassenTeller(string s, ref int teller)
        {
            if (teller > 1)
                throw new Exception("Meer dan 1 klasse in de klasse file.");
            if (s.Contains("class") || s.Contains("interface"))
                teller++;
        }
        public void CodeTeller(string s, ref int teller)
        {
            if (s.Length > 2)
                teller++;
        }
    }
}
