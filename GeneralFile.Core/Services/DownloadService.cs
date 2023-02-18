using GeneralFile.Core.Exceptions;
using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace GeneralFile.Core.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly IFileDownloadService _fileDownloadService;
        private readonly ICoreService _coreService;
        public DownloadService(IFileDownloadService fileDownloadService, ICoreService coreService)
        {
            _fileDownloadService = fileDownloadService;
            _coreService = coreService;
        }

        public async Task<FileDownload> DownloadFile(string usuario, string ruta)
        {
            ruta = _fileDownloadService.ReplaceBackSlashes(ruta);

            if (await _fileDownloadService.HasPermissions(ruta, usuario))
                return await _fileDownloadService.DownloadSingleFile(ruta);
            else
                throw new BusinessException("El usuario no tiene permisos sobre este archivo");
        }


        public async Task<FileDownload> DownloadFiles(string usuario, [FromQuery] string rutas)
        {
            rutas = rutas.Replace(" ", "");
            bool pathContains = rutas.Contains("\\");
            if (pathContains)
                rutas = rutas.Replace("\\", "/");
            IEnumerable<string> ListRutas = rutas.Split(',').ToList();
            List<string> filesWithPermissions = await _fileDownloadService.GetFilesWithUserPermission(ListRutas,usuario);
            if (filesWithPermissions.Count > 0)
            {
                FileDownload filesDownload = await _fileDownloadService.CompressFilesToZipAsync(filesWithPermissions);
                return filesDownload;  
            }
            else
                throw new BusinessException("El usuario no tiene permisos sobre estos archivos");
        }


        public async Task<FileDownload> DownloadFilesFolder(string usuario, string rutaCarpeta)
        {            
            List<string> filesWithPermissions = await _coreService.getFilesByUser(rutaCarpeta, usuario);
            List<string> filestodownloadList = await _fileDownloadService.GetfilestodownloadList(filesWithPermissions);

            if (filestodownloadList.Count() > 0)
            {
                FileDownload filesDownload = await _fileDownloadService.CompressFilesToZipAsync(filestodownloadList);
                return filesDownload;
            }
            else
                throw new BusinessException("El usuario no tiene permisos sobre ningún archivo de estar ruta");
        }
    }
}
