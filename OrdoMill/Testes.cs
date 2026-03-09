using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Moq;
using NUnit.Framework;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using OrdoMill.Views.Bordereau;

namespace OrdoMill
{
    [TestFixture]
    internal static class Testes
    {
        [SetUp]
        public static void SetUp()
        {
            Db = new DbCon("DbCon");
            // Database.SetInitializer(new CreateDatabaseIfNotExists<DbCon>());
        }

        public static DbCon Db { get; private set; }

        public static void Msg(this object msg)
        {
            MessageBox.Show(msg?.ToString());
        }

        private static readonly Random Rundom = new Random(100);

        [Time]
        private static List<MedOrd> GetMedicaments()
        {
            var result = new List<MedOrd>();
            for (var i = 0; i < Rundom.Next(1, 7); i++)
            {
                var medOrd = new MedOrd
                {
                    MedicamentId = Rundom.Next(1, 2000),
                    Quantite = Rundom.Next(1, 4),
                    Ppa = Rundom.NextDouble() + Rundom.Next(100, 4000)
                };
                result.Add(medOrd);
            }
            return result;
        }

        [Test(Author = "Okba", Description = "Test if computed property working ")]
        public static void ComputedPropTest()
        {
            var a = Db?.Assures?.Select(assure => assure.FullName)?.FirstOrDefault();
            Msg($"Full Name is: {a}");
            Assert.IsNotEmpty(a);
        }

        [Test]
        public static async Task CreateOrdonnanceForTesting()
        {
            var a = 0;
            try
            {
                var fl = Db.Factures.Count(x => x.Ordonnances.Count < 50);

                for (var I = 0; I < 150; I++)
                {
                    var ord = new Ordonnance
                    {
                        PatientId = Rundom.Next(1, 500),
                        FactureId = Rundom.Next(1, fl),
                        MedecinId = Rundom.Next(1, 7),
                        Medicaments = GetMedicaments(),
                        ServiDate = DateTime.Today,
                        SoineDate = DateTime.Today.AddDays(-2)
                    };
                    Db.Ordonnances.Add(ord);
                }
                a = await Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Msg("Error\n" + ex);
            }
            finally
            {
                Msg(a);
            }
        }

        [Test]
        public static void DeleteUsersTest()
        {
            foreach (var user in Db.Users)
                Db.Users.Remove(user);
            var i = Db.SaveChanges();
            Msg($"Delete {i} Users");
            Assert.IsTrue(true);
        }

        [Test][Time]
        public static async Task ExtractionTest()
        {
            try
            {
                var factur = await FacturesAndBordereauService.GetAllFactureInfosAsync(1);
                var info = Db.Infos.FirstOrDefault();
                var progress = new ActionProgress<int>();
                var result = await Task.Run(() => factur.ExtractFactureToExcel("D:\\", info, progress));
                Assert.True(result);
             }
            catch (Exception ex)
            {
                Msg(ex);
            }
        }

        [Test(Author = "Nadir", Description = "TestToAddUsers")]
        public static void InsertUserTest()
        {
            var a =
                Db.Users.Add(new User
                {
                    UserName = "Nadir",
                    FullName = "Mezhoudi Hadj Nadir",
                    IsWork = true,
                    IsAdmin = true,
                    Password = "1"
                });
            var b =
                Db.Users.Add(new User
                {
                    UserName = "Admin",
                    FullName = "Mezhoudi Hadj Nadir",
                    IsWork = true,
                    IsAdmin = true,
                    Password = "1"
                });
            var i = Db.SaveChanges();
            Msg($"Insert {i} Users");
            Assert.AreEqual(i, 2);
        }

        [Test]
        public static void TestMoq()
        {
            var a = new Mock<Assure>().Object;
            Msg(a.Nom);
        }

        [Test]
        public static void UsersCountTest()
        {
            var i = Db.Users.Count();
            $"Users Count = {i} ".Msg();
            Assert.NotZero(i);
        }
    }
}