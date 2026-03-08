using System;
using System.Data.Entity;
using System.Threading.Tasks;
using OrdoMill.Data.Model;
using OrdoMill.Properties;

namespace OrdoMill.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbCon _context;
        private bool _disposed = false;

        public UnitOfWork()
        {
            _context = new DbCon(Settings.Default.ConnectionString);
        }

        public UnitOfWork(string connectionString)
        {
            _context = new DbCon(connectionString);
        }

        public DbSet<Assure> Assures => _context.Assures;
        public DbSet<Bordereau> Bordereaus => _context.Bordereaus;
        public DbSet<Facture> Factures => _context.Factures;
        public DbSet<Forme> Formes => _context.Formes;
        public DbSet<Historique> Historiques => _context.Historiques;
        public DbSet<Info> Infos => _context.Infos;
        public DbSet<Medecin> Medecins => _context.Medecins;
        public DbSet<Medicament> Medicaments => _context.Medicaments;
        public DbSet<MedOrd> MedOrds => _context.MedOrds;
        public DbSet<Operation> Operations => _context.Operations;
        public DbSet<Ordonnance> Ordonnances => _context.Ordonnances;
        public DbSet<Pathologie> Pathologies => _context.Pathologies;
        public DbSet<Patient> Patients => _context.Patients;
        public DbSet<User> Users => _context.Users;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public DbCon GetContext()
        {
            return _context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
