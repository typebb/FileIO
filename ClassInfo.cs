using System;
using System.Collections.Generic;
using System.Text;

namespace FileIO
{
    public class ClassInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string FirstMethod { get; set; }
        public HashSet<string> Constructors { get; set; } = new HashSet<string>();
        public HashSet<string> Methods { get; set; } = new HashSet<string>();
        public HashSet<string> Inherits { get; set; } = new HashSet<string>();
        public HashSet<string> Usings { get; set; } = new HashSet<string>();
        public HashSet<string> Properties { get; set; } = new HashSet<string>();
        public HashSet<string> Variables { get; set; } = new HashSet<string>();
        public override string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append($"{Namespace},{Name}\n");
            foreach (string s in Usings) if (s != "") returnString.Append($"Using : {s}\n");
            foreach (string s in Inherits) if (s != "") returnString.Append($"Inherit : {s}\n");
            foreach (string s in Constructors) if (s != "") returnString.Append($"Constructor : {s}\n");
            foreach (string s in Methods) if (s != "") returnString.Append($"Method : {s}\n");
            foreach (string s in Properties) if (s != "") returnString.Append($"Property : {s}\n");
            foreach (string s in Variables) if (s != "") returnString.Append($"Variable : {s}\n");
            returnString.Append("_______________________\n");
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
