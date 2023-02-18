using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using System.Net.Http.Json;

namespace GeneralFile.Core.Services
{
    public class LogService : ILogService
    {
        private readonly Settings _settings;

        private static string? _genral_service_url;

        HttpClient httpClient = new HttpClient();

        public LogService(Settings settings)
        {
            _settings = settings;
            _genral_service_url = _settings.generalService;
        }
        public async Task CreateLog(Log log)
        {
            await httpClient.PostAsJsonAsync(_genral_service_url + "Log/PostLogService", log);
        }
    }
}
