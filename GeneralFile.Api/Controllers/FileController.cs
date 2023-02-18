using GeneralFile.Core.Exceptions;
using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using Microsoft.AspNetCore.Mvc;


namespace GeneralFile.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ICoreService _coreService;
        private readonly IDownloadService _downloadService;
        private readonly ILogService _logService;

        public FileController(ICoreService coreService, IDownloadService downloadService, ILogService logService)
        {           
            _coreService = coreService;        
            _downloadService = downloadService;
            _logService = logService;
        }

        /// <summary>
        /// Listar Archivo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ListarArchivos")]
        public async Task<IActionResult> ListarArchivos(string usuario, string ruta)
        {
            try
            {
                List<string> files = await _coreService.getFilesByUser(ruta, usuario);
                if(files.Count > 0)
                    return Ok(files);
                else
                    return BadRequest("El usuario no tiene permisos sobre ningún archivo de estar ruta");
            }
            catch (Exception ex)
            {
                Log log = new Log() { Service = "General File", Message = "Error: " + ex.GetType().ToString() + " , " + ex.Message, Date = DateTime.Now };
                await _logService.CreateLog(log);
                throw new BusinessException(ex.Message);
            }
        }

        /// <summary>
        /// Descargar Archivo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DescargarArchivo")]
        public async Task<IActionResult> DescargarArchivo(string usuario, string ruta)
        {
            try
            {
                FileDownload fileDownload = await _downloadService.DownloadFile(usuario, ruta);
                return File(fileDownload.Stream.ToArray(), fileDownload.ContentType, fileDownload.Name);
            }
            catch (Exception ex)
            {
                Log log = new Log() { Service = "General File", Message = "Error: " + ex.GetType().ToString() + " , " + ex.Message, Date = DateTime.Now };
                await _logService.CreateLog(log);
                throw new BusinessException(ex.Message);
            }
        }


        /// <summary>
        /// Descargar Archivos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DescargarArchivos")]
        public async Task<IActionResult> DescargarArchivos(string usuario, [FromQuery]string rutas)
        {
            try
            {
                FileDownload filesDownload = await _downloadService.DownloadFiles(usuario, rutas);
                return File(filesDownload.Stream.ToArray(), "application/zip", "Archivos.zip");
            }
            catch (Exception ex)
            {
                Log log = new Log() { Service = "General File", Message = "Error: " + ex.GetType().ToString() + " , " + ex.Message, Date = DateTime.Now };
                await _logService.CreateLog(log);
                throw new BusinessException(ex.Message);
            }            
        }

        /// <summary>
        /// Descargar Archivos Por Carpeta
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DescargarArchivosCarpeta")]
        public async Task<IActionResult> DescargarArchivosCarpeta(string usuario, string rutaCarpeta)
        {            
            try
            {
                FileDownload filesDownload = await _downloadService.DownloadFilesFolder(usuario, rutaCarpeta);
                return File(filesDownload.Stream.ToArray(), "application/zip", rutaCarpeta + ".zip");    
            }
            catch (Exception ex)
            {
                Log log = new Log() { Service = "General File", Message = "Error: " + ex.GetType().ToString() + " , " + ex.Message, Date = DateTime.Now };
                await _logService.CreateLog(log);
                throw new BusinessException(ex.Message);
            }
        }
    }
}
