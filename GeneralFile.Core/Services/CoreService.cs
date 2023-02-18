using GeneralFile.Core.Interfaces;
using GeneralFile.Core.Model;
using System.Text.Json;

namespace GeneralFile.Core.Services
{
    public class CoreService: ICoreService
    {
        private static string? _core_service_url;

        private readonly Settings _settings;

        HttpClient httpClient = new HttpClient();

        JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public CoreService(Settings settings)
        {
            _settings = settings;
            _core_service_url = _settings.coreUrl;
        }
        public async Task<List<string>> getFilesByUser(string path,string user)
        {
            var response = await httpClient.GetAsync(_core_service_url + "getFilesByUser?path="+path+ "&user="+user);          
            var content = await response.Content.ReadAsStringAsync();
            List<string> files = JsonSerializer.Deserialize<List<string>>(content, options);
            return files;
        }
    }
}
