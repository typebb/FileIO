using System.IO;

namespace FileIO
{
    public class OutputBestand
    {
        public string OutputFolder { get; set; }
        public OutputBestand()
        {
            OutputFolder = Path.Combine(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().LastIndexOf("FileIO") + 6), "Output");
            Directory.CreateDirectory(OutputFolder);
        }
        public void CreateNewDirectory(string naam)
        {
            Directory.CreateDirectory(Path.Combine(OutputFolder, naam));
        }
        public void WriteOutputToFile(string foldernaam, BestandInfo output)
        {
            string path = Path.Combine(OutputFolder, foldernaam, $"Analyse{foldernaam}");
            File.AppendAllText(path, output.ToString());
        }
    }
}
