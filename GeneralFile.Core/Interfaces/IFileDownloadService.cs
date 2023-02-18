using Azure.Storage.Blobs.Models;
using GeneralFile.Core.Model;

namespace GeneralFile.Core.Interfaces
{
    public interface IFileDownloadService
    {
        Task<FileDownload> DownloadSingleFile(string ruta);
        Task<bool> HasPermissions(string ruta, string usuario);

        string ReplaceBackSlashes(string path);

        Task<List<string>> GetFilesWithUserPermission(IEnumerable<string> ListRutas, string usuario);

        Task<FileDownload> CompressFilesToZipAsync(List<string> filesWithPermissions);

        Task AddFileToZipArchiveAsync(string filePath, System.IO.Compression.ZipArchive archive, MemoryStream stream);

        Task<List<string>> GetfilestodownloadList(List<string> filesWithPermissions);

        Task<string?>  GetBlobNameWithpermissions(List<string> filesWithPermissions, BlobItem blob);
    }
}
