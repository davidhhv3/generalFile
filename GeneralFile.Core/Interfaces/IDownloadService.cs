using GeneralFile.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace GeneralFile.Core.Interfaces
{
    public interface IDownloadService
    {
        Task<FileDownload> DownloadFile(string usuario, string ruta);

        Task<FileDownload> DownloadFiles(string usuario, [FromQuery] string rutas);
        Task<FileDownload> DownloadFilesFolder(string usuario, string rutaCarpeta);
    }
}
