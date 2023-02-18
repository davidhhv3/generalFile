using GeneralFile.Core.Model;

namespace GeneralFile.Core.Interfaces
{
    public interface ILogService
    {
        Task CreateLog(Log log);
    }
}
