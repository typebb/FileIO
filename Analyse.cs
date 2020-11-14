namespace FileIO
{
    public class Analyse
    {
        private AnalyseBestanden AnalyseBestanden { get; set; }
        private AnalyseKlassen AnalyseKlassen { get; set; }
        public Analyse()
        {
            AnalyseBestanden = new AnalyseBestanden();
            AnalyseKlassen = new AnalyseKlassen();
        }
        public void VoerAnalyseUit()
        {
            AnalyseBestanden.IterateFoldersAndFiles();
            AnalyseKlassen.IterateFoldersAndFiles();
        }
    }
}
