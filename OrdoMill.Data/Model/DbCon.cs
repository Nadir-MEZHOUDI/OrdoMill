using Microsoft.EntityFrameworkCore;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Data.Model
{
    public sealed class DbCon : DbContext
    {
        private readonly string _connectionString;

        public DbCon()
        {
        }

        public DbCon(string dbCon)
        {
            _connectionString = dbCon;
        }

        public async Task AddOrUpdate<T>(T entity) where T : EntityBase
        {
            if (entity == null)
            {
                return;
            }

            if (entity.Id > 0)
                Entry(entity).State = EntityState.Modified;
            else
                Set<T>().Add(entity);
            await SaveChangesAsync();

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && !string.IsNullOrWhiteSpace(_connectionString))
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

    }
}
