namespace LibgenUI
{
    public class Document
    {
        public string Hash { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public long SizeBytes { get; set; }
        public string FilePath { get; set; }
        public Document(string hash, string title, string author, long sizeBytes, string filePath)
        {
            Hash = hash;
            Title = title;
            Author = author;
            SizeBytes = sizeBytes;
            FilePath = filePath;
        }
    }
}
