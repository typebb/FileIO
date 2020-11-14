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
        public void WriteOutputToFile(string foldernaam, List<ClassInfo> output, string soort)
        {
            string path = Path.Combine(OutputFolder, foldernaam, $"{soort}{foldernaam}");
            foreach(ClassInfo c in output)
            File.AppendAllText(path, c.ToString());
        }
    }
}
