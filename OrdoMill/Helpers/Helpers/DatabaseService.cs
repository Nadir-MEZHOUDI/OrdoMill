using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace OrdoMill.Helpers
{
    public class DatabaseService 
    {
            

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
