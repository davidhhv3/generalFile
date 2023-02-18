using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using System.IO.Compression;

namespace GeneralFile.Core.Services
{
    public class FileDownloadService: IFileDownloadService
    {
        private readonly Settings _settings;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ICoreService _coreService;

        public FileDownloadService(Settings settings, ICoreService coreService)
        {
            _settings = settings;
            _coreService = coreService;
            _blobContainerClient = new BlobContainerClient(_settings.blobConexion, _settings.blobContainer);
        }

        public async Task<FileDownload> DownloadSingleFile(string ruta)
        {
            var client = _blobContainerClient.GetBlobClient(ruta);

            using (var stream = new MemoryStream())
            {
                await client.DownloadToAsync(stream);
                string name = client.Name.Split('/').Last();
                stream.Position = 0;
                string contentType = (await client.GetPropertiesAsync()).Value.ContentType;
                FileDownload fileDownload = new FileDownload() { Stream = stream, ContentType = contentType, Name = name };
                return fileDownload;
            }
        }

        public async Task<bool> HasPermissions(string ruta, string usuario)
        {
            List<string> fileWithPermissions = await _coreService.getFilesByUser(ruta, usuario);
            return fileWithPermissions.Count > 0;
        }

        public string ReplaceBackSlashes(string path)
        {
            bool pathContains = path.Contains("\\");
            if (pathContains)
                path = path.Replace("\\", "/");
            return path;
        }
        public async Task<List<string>> GetFilesWithUserPermission(IEnumerable<string> ListRutas, string usuario)
        {
            List<string> filesWithPermissions = new List<string>();
            foreach (var ruta in ListRutas)
            {
                List<string> fileWithPermissions = await _coreService.getFilesByUser(ruta, usuario);
                if (fileWithPermissions.Count > 0)
                    filesWithPermissions.Add(ruta);
            }
            return filesWithPermissions;
        }
        public async Task<FileDownload> CompressFilesToZipAsync(List<string> filesWithPermissions)
        {
            using (var stream = new MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in filesWithPermissions)
                    {
                        await AddFileToZipArchiveAsync(item, archive, stream);
                    }
                }
                FileDownload filesDownload = new FileDownload() { Stream = stream, ContentType = "", Name = "" };
                return filesDownload;
            }
        }
        public async Task AddFileToZipArchiveAsync(string filePath, System.IO.Compression.ZipArchive archive, MemoryStream stream)
        {
            var client = _blobContainerClient.GetBlobClient(filePath);

            var downloadFile = await client.DownloadToAsync(stream);
            byte[] bytes = stream.ToArray();
            var name = client.Name.Split('/').Last();

            var zipEntry = archive.CreateEntry(name, CompressionLevel.Fastest);

            using (var zipStream = zipEntry.Open())
            {
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }
        public async Task<List<string>> GetfilestodownloadList(List<string> filesWithPermissions)
        {
            List<string> filestodownloadList = new List<string>();
            var blobs = _blobContainerClient.GetBlobs();
            foreach (var blob in blobs)
            {
                string? blobName = await GetBlobNameWithpermissions(filesWithPermissions, blob);
                if (blobName != null)
                    filestodownloadList.Add(blobName);
            }
            return filestodownloadList;
        }

        public async Task<string?> GetBlobNameWithpermissions(List<string> filesWithPermissions, BlobItem blob)
        {
            foreach (var archivo in filesWithPermissions)
            {
                string rutaArchivo = (archivo).Replace("\\", "/");
                if (blob.Name == rutaArchivo)
                    return blob.Name;
            }
            return null;
        }
    }
}
