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
            OutputFolder = Path.Combine(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().LastIndexOf("FileIO") + 6), "Output");
            Directory.CreateDirectory(OutputFolder);
        }
        public void CreateNewDirectory(string naam)
        {
            Directory.CreateDirectory(Path.Combine(OutputFolder, naam));
        }
        public void WriteOutputToFile(string foldernaam, List<string> output, string soort)
        {
            string path = Path.Combine(OutputFolder, foldernaam, $"{soort}{foldernaam}");
            File.AppendAllLines(path, output);
        }
    }
}
