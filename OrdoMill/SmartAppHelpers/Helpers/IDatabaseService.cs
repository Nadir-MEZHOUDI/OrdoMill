using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartApp.Helpers.Helpers
{
    public interface IDatabaseService
    {
        Task<IEnumerable<string>> GetDatabasesFromServerAsync(string serverName);

        Task<IEnumerable<string>> GetSqlServerInstancesAsync(string machineName);

        Task<IEnumerable<string>> GetAllPCsOnLocalNetworkAsync();
    }
}
