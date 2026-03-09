using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using OrdoMill.Properties;

namespace OrdoMill.Helpers
{
    public class DatabaseService : IDatabaseService
    {
        public async Task<IEnumerable<string>> GetDatabasesFromServerAsync(string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                return Array.Empty<string>();
            }

            var builder = new NpgsqlConnectionStringBuilder();

            if (!string.IsNullOrWhiteSpace(Settings.Default.ConnectionString))
            {
                try
                {
                    builder.ConnectionString = Settings.Default.ConnectionString;
                }
                catch
                {
                }
            }

            builder.Host = serverName;
            builder.Port = builder.Port == 0 ? 5432 : builder.Port;
            builder.Database = string.IsNullOrWhiteSpace(builder.Database) ? "postgres" : builder.Database;

            await using var connection = new NpgsqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            const string sql = "SELECT datname FROM pg_database WHERE datistemplate = false ORDER BY datname;";
            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();
            var databases = new List<string>();

            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }

            return databases;
        }

        public Task<IEnumerable<string>> GetSqlServerInstancesAsync(string machineName)
        {
            IEnumerable<string> hosts = new[]
            {
                "localhost",
                "127.0.0.1",
                string.IsNullOrWhiteSpace(machineName) ? Environment.MachineName : machineName
            }.Distinct(StringComparer.OrdinalIgnoreCase);

            return Task.FromResult(hosts);
        }

        public async Task<IEnumerable<string>> GetAllPCsOnLocalNetworkAsync() => await Task.Run(() =>
        {
            var pCs = new List<string>();
            try
            {
                var root = new DirectoryEntry("WinNT:");
                foreach (DirectoryEntry computers in root.Children)
                {
                    pCs.AddRange(from DirectoryEntry computer in computers.Children where computer.Name != "Schema" select computer.Name);
                }
            }
            catch (Exception)
            {
                throw new ApplicationException(@"��� ��� ����� ����� �� ������� ������� ���� �������� ��� ������ �� ������");
            }

            return pCs;
        });
    }
}
