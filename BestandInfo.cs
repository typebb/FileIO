using System;
using System.Collections.Generic;
using System.Text;

namespace FileIO
{
    public class BestandInfo
    {
        public string TypeOfClass { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public int AantalLijnenCode { get; set; }
        public override string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.Append($"Namespace : {Namespace}\n");
            returnString.Append($"Class naam : {Name}\n");
            returnString.Append($"Soort van Class : {TypeOfClass}\n");
            returnString.Append($"Aantal lijnen code : {AantalLijnenCode}\n");
            returnString.Append("_______________________\n");
            return returnString.ToString();
        }
        public void Show()
        {
            Console.WriteLine($"Namespace : {Namespace}");
            Console.WriteLine($"Class naam : {Name}");
            Console.WriteLine($"Soort van Class : {TypeOfClass}");
            Console.WriteLine($"Aantal lijnen code : {AantalLijnenCode}");
            Console.WriteLine("_______________________");
        }
    }
}
