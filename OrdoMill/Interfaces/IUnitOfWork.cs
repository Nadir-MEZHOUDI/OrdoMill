using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using OrdoMill.Data.Model;

namespace OrdoMill.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<Assure> Assures { get; }
        DbSet<Bordereau> Bordereaus { get; }
        DbSet<Facture> Factures { get; }
        DbSet<Forme> Formes { get; }
        DbSet<Historique> Historiques { get; }
        DbSet<Info> Infos { get; }
        DbSet<Medecin> Medecins { get; }
        DbSet<Medicament> Medicaments { get; }
        DbSet<MedOrd> MedOrds { get; }
        DbSet<Operation> Operations { get; }
        DbSet<Ordonnance> Ordonnances { get; }
        DbSet<Pathologie> Pathologies { get; }
        DbSet<Patient> Patients { get; }
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
        DbCon GetContext();
    }
}
