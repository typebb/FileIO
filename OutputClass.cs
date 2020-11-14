using System.IO;

namespace FileIO
{
    public class OutputClass
    {
        public string OutputFolder { get; set; }
        public OutputClass()
        {
            OutputFolder = Path.Combine(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().LastIndexOf("FileIO") + 6), "Output");
            Directory.CreateDirectory(OutputFolder);
        }
        public void CreateNewDirectory(string naam)
        {
            Directory.CreateDirectory(Path.Combine(OutputFolder, naam));
        }
        public void WriteOutputToFile(string foldernaam, ClassInfo output)
        {
            string path = Path.Combine(OutputFolder, foldernaam, $"ClassInfo{foldernaam}");
            File.AppendAllText(path, output.ToString());
        }
    }
}
