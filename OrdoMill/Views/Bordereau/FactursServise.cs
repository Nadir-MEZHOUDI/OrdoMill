using Microsoft.EntityFrameworkCore;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Services;

namespace OrdoMill.Views.Bordereau;

internal class FacturesAndBordereauService
{
     public static async Task<Facture> GetAllFactureInfosAsync(int id)
    {
        try
        {
            // using (var db = new DbCon(Settings.Default.ConnectionString))
            using var db = new DbCon(Settings.Default.ConnectionString);
            var factur = await db.Factures
                .Include(facture => facture.Ordonnances.Select(a => a.Medecin))
                .Include(facture => facture.Ordonnances.Select(o => o.Patient))
                .Include(facture => facture.Ordonnances.Select(o => o.Patient.Assure))
                .Include(x => x.Ordonnances.Select(a => a.Medicaments.Select(ord => ord.Medicament.Forme)))
                .FirstOrDefaultAsync(f => f.Id == id);
            return factur;
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
        return null;
    }

    internal static async Task<Data.Model.Bordereau> GetFullBordereauInfo(int id)
    {
        try
        {
            // using (var db = new DbCon(Settings.Default.ConnectionString))
            using var db = new DbCon(Settings.Default.ConnectionString);
            var bordereau = await db.Bordereaus.Include(b => b.Factures.Select(f => f.Ordonnances.Select(o => o.Medecin)))
                .Include(b => b.Factures.Select(facture => facture.Ordonnances.Select(ordonnance => ordonnance.Medicaments.Select(ord => ord.Medicament.Forme))))
                .Include(b => b.Factures.Select(f => f.Ordonnances.Select(o => o.Patient.Assure)))
                .FirstOrDefaultAsync(x => x.Id == id);
            return bordereau;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
