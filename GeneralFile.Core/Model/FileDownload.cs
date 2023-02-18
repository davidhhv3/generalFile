namespace GeneralFile.Core.Model
{
    public class FileDownload
    {
        public MemoryStream? Stream { get; set; }   
        
        public string? ContentType { get; set; } 

        public string? Name { get; set; }

    }
}
