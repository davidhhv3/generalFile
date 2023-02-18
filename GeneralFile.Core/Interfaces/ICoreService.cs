namespace GeneralFile.Core.Interfaces
{
    public interface ICoreService
    {       Task<List<string>> getFilesByUser(string path, string user);
    }
}
