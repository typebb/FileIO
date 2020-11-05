using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileIO
{
    public class Output
    {
        public string OutputFolder { get; set; }
        public Output()
        {
            //Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Output"));
            OutputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Output");
        }
        public void CreateNewDirectory(string naam)
        {
            Directory.CreateDirectory(Path.Combine(OutputFolder, naam));
        }
        public void WriteOutputToFile(string foldernaam, string filenaam, List<string> output)
        {
            string path = Path.Combine(OutputFolder, foldernaam, filenaam, "Analyse");
            if (!File.Exists(path))
                File.WriteAllLines(path, output);
        }
    }
}
