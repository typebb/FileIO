using System;
using System.Collections.Generic;
using System.Text;

namespace FileIO
{
    public class Analyse
    {
        private AnalyseBestanden AnalyseBestanden { get; set; }
        private AnalyseKlassen AnalyseKlassen { get; set; }
        public Analyse()
        {
            Input input = new Input();
            Output output = new Output();
            AnalyseBestanden = new AnalyseBestanden();
            AnalyseKlassen = new AnalyseKlassen();
            AnalyseBestanden.Input = input;
            AnalyseBestanden.Output = output;
            AnalyseKlassen.Input = input;
            AnalyseKlassen.Output = output;
        }
        public void VoerAnalyseUit()
        {
            AnalyseBestanden.IterateFoldersAndFiles();
            AnalyseKlassen.IterateFoldersAndFiles();
        }
    }
}
