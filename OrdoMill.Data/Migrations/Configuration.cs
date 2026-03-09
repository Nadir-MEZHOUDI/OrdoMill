using System;
using System.Linq;
using OrdoMill.Data.Model;

namespace OrdoMill.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<DbCon>
    {
        public Configuration()
        {          
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DbCon context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            try
            {
                if (context?.Operations?.Count() < 9)
                {
                    context.Operations.AddOrUpdate(o => o.Name,
                        new Operation { Name = "������� ����� ��������" },
                        new Operation { Name = "����� ������" },
                        new Operation { Name = "����� ������" },
                        new Operation { Name = "������� ����� ���������" },
                        new Operation { Name = "����� ����� ���������" },
                        new Operation { Name = "����� ����� �����" },
                        new Operation { Name = "����� ����� �����" },
                        new Operation { Name = " ����� �����" },
                        new Operation { Name = "����� �����" }
                    );
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                // throw new ApplicationException("��� ��� �� ����� ��������\n" + ex.Message);
            }

            try
            {
                if (context?.Users?.Any(x => x.IsAdmin) == false)
                {
                    var admin = new User
                    {
                        UserName = "Admin",
                        FullName = "Admin",
                        Password = "admin",
                        IsAdmin = true,
                        IsWork = true,
                        AllowAdd = true,
                        AllowPrintDocs = true,
                        AllowUpdate = true
                    };

                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }

            catch(Exception)
            {
                //
             }

        }
    }
}
