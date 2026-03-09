using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace OrdoMill.Helpers
{
    public class DatabaseService : IDatabaseService
    {
        public async Task<IEnumerable<string>> GetDatabasesFromServerAsync(string serverName) => await Task.Run(() =>
        {
            try
            {
                var databases = new List<string>();
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = serverName,
                    InitialCatalog = "master",
                    IntegratedSecurity = true,
                    TrustServerCertificate = true
                };

                using var connection = new SqlConnection(builder.ConnectionString);
                using var command = new SqlCommand("SELECT [name] FROM sys.databases ORDER BY [name]", connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    databases.Add(reader.GetString(0));
                }

                return databases;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        });

        public async Task<IEnumerable<string>> GetSqlServerInstancesAsync(string machineName) => await Task.Run(() =>
        {
            var list = new List<string>();
            try
            {
                string baseName = string.IsNullOrWhiteSpace(machineName) ? Environment.MachineName : machineName;

                list.Add(baseName);
                list.Add(baseName + @"\SQLEXPRESS");
                list.Add(baseName + @"\MSSQLSERVER");

                if (baseName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(@"(localdb)\MSSQLLocalDB");
                }

                return list
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception)
            {
                throw new ApplicationException(@"��� ��� �� ��� ���� ����� �������� ���� ������ �� ����� SQL Server ");
            }
        });

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
