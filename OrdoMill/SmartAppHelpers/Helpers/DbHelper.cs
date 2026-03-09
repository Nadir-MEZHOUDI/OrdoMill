using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartApp.Helpers.Helpers
{
    public static class DbHelper
    {
        public static async Task<bool> CheckDbConnectionAsync(string conStr) => await Task.Run(() =>
        {
            var con = new SqlConnection(conStr);
            try
            {
                con.Open();
                return con.State == ConnectionState.Open;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        });

        public static bool CheckDbConnection(string conStr)
        {
            var con = new SqlConnection(conStr);
            try
            {
                con.Open();
                return con.State == ConnectionState.Open;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public static bool SaveBackup(string backUpPath, string dbName, string connectionString)
        {
            var file = new FileInfo(backUpPath);
            string fullName = file.Directory?.FullName ?? new FileInfo(@"Backups\").FullName;
            if (Directory.Exists(fullName))
            {
                Directory.CreateDirectory(fullName);
            }

            var conn = new SqlConnection(connectionString);
            try
            {
                var cmd = new SqlCommand($"BACKUP DATABASE {dbName} TO DISK = '{backUpPath}'") { Connection = conn };
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(@"��� ��� ����� ��� ������ ����������" + Environment.NewLine + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool RestorDb(string fromPath, string dbName, string serverName)
        {
            var conn = new SqlConnection($"Server={serverName};Database=master;Integrated Security=True;");
            try
            {
                string a =
                    $"RESTORE FILELISTONLY FROM DISK ='{fromPath}' ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE RESTORE DATABASE {dbName} FROM DISK ='{fromPath}' WITH REPLACE ALTER DATABASE {dbName} SET MULTI_USER ";
                var cmd = new SqlCommand(a, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("\n ��� ��� ����� ����� ������� ����� ��������" + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public static void RefreshEntry(this DbContext context)
        {
            try
            {
                var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
                foreach (DbEntityEntry dbEntityEntry in entries)
                {
                    dbEntityEntry.Reload();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
