using System.Data.Entity;
using OrdoMill.Data.Migrations;

namespace OrdoMill.Data.Model
{
    public sealed class DbCon : DbContext
    {
        public DbCon() : this("DbCon")
        {
        }
        public DbCon(string dbPath) : base(dbPath)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbCon, Configuration>(true));
        }

        public DbSet<Assure> Assures { get; set; }
        public DbSet<Bordereau> Bordereaus { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Forme> Formes { get; set; }
        public DbSet<Historique> Historiques { get; set; }
        public DbSet<Info> Infos { get; set; }
        public DbSet<Medecin> Medecins { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<MedOrd> MedOrds { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Ordonnance> Ordonnances { get; set; }
        public DbSet<Pathologie> Pathologies { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }


    }
}