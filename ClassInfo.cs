using System;
using System.Collections.Generic;
using System.Text;

namespace FileIO
{
    public class ClassInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public List<string> Constructors { get; set; } = new List<string>();
        public List<string> Methods { get; set; } = new List<string>();
        public List<string> Inherits { get; set; } = new List<string>();
        public List<string> Usings { get; set; } = new List<string>();
        public List<string> Properties { get; set; } = new List<string>();
        public List<string> Variables { get; set; } = new List<string>();
        public override string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append($"{Namespace},{Name}\n");
            foreach (string s in Usings) returnString.Append($"Using : {s}\n");
            foreach (string s in Inherits) returnString.Append($"Inherit : {s}\n");
            foreach (string s in Constructors) returnString.Append($"Constructor : {s}\n");
            foreach (string s in Methods) returnString.Append($"Method : {s}\n");
            foreach (string s in Properties) returnString.Append($"Property : {s}\n");
            foreach (string s in Variables) returnString.Append($"Variable : {s}\n");
            returnString.Append("_______________________");
            return returnString.ToString();
        }
        public void Show()
        {
            Console.WriteLine($"{Namespace},{Name}");
            foreach (string s in Constructors) Console.WriteLine($"Constructors : {s}");
            foreach (string s in Methods) Console.WriteLine($"Methods : {s}");
            foreach (string s in Inherits) Console.WriteLine($"Inherits : {s}");
            foreach (string s in Usings) Console.WriteLine($"Usings : {s}");
            foreach (string s in Properties) Console.WriteLine($"Properties : {s}");
            foreach (string s in Variables) Console.WriteLine($"Parameters : {s}");
            Console.WriteLine("_______________________");
        }
    }
}
