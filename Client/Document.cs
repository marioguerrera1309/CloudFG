namespace LibgenUI
{
    public class Document
    {
        public string? Hash { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Date { get; set; }
        public long SizeBytes { get; set; }
        public string? FilePath { get; set; }
        //Questo dice a C# che è accettabile che quei valori siano 
        //inizialmente vuoti mentre il sistema "spacchetta" il JSON che arriva da Go.

        public Document() { }  //costruttore vuoto necessario per il json
        public Document(string hash, string title, string author, string date, long sizeBytes, string filePath)
        {
            Hash = hash;
            Title = title;
            Author = author;
            Date = date;
            SizeBytes = sizeBytes;
            FilePath = filePath;
        }
    }
}
