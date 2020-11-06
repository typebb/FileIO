using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileIO
{
    public class AnalyseKlassen
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
                    Output.WriteOutputToFile(folderPath, Path.GetFileNameWithoutExtension(f), output);
                }
            }
        }
        public void Analyseer(string path, ref List<string> output)
        {
            foreach (string s in File.ReadLines(path))
            {
                CheckStringAndAddToList(s, ref output);
            }
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
                    s = s.Substring(s.IndexOf("class"), s.IndexOf(":") - 2);
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
