using System.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Npgsql;

namespace OrdoMill.Helpers;

public static class DbHelper
{
    public static async Task<bool> CheckDbConnectionAsync(string conStr) => await Task.Run(() =>
    {
        var con = new NpgsqlConnection(conStr);
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
        var con = new NpgsqlConnection(conStr);
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

         try
        {
           //add Postgres backup code here

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(@"��� ��� ����� ��� ������ ����������" + Environment.NewLine + ex.Message);
        }
        finally
        {

        }
    }

    public static bool RestorDb(string fromPath, string dbName, string serverName)
    {
         try
        {
           //Add Postgres restore code here
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("\n ��� ��� ����� ����� ������� ����� ��������" + ex.Message);
        }
        finally
        {
             
        }
    }

    public static void RefreshEntry(this DbContext context)
    {
        try
        {
            var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
            foreach (var dbEntityEntry in entries)
            {
                dbEntityEntry.Reload();
            }
        }
        catch (Exception)
        {
        }
    }
}
