using Azure.Storage.Blobs;
using GeneralFile.Core.Model;

namespace GeneralFile.Api.Helpers
{
    public class MultipleDownloadBlob
    {
        private readonly Settings _settings;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly BlobServiceClient _blobServiceClient;
        public MultipleDownloadBlob(Settings settings, BlobServiceClient blobServiceClient)
        {
            _settings = settings;
            _blobContainerClient = new BlobContainerClient(_settings.blobConexion, _settings.blobContainer);
            _blobServiceClient = blobServiceClient;
        }

        public async Task DownloadMultipleFilesUsingParallelForEachAsync(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobPages = blobContainerClient.GetBlobsAsync().AsPages();
            await Parallel.ForEachAsync(blobPages, new ParallelOptions { MaxDegreeOfParallelism = 2 }, async (blobPage, token) =>
            {
                await Parallel.ForEachAsync(blobPage.Values, new ParallelOptions { MaxDegreeOfParallelism = 2 }, async (blob, token) =>
                {
                    var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                    using var file = File.Create(blob.Name);
                    await blobClient.DownloadToAsync(file);
                });
            });
        }
    }
}
