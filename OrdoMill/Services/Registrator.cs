using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.ViewModel;

namespace OrdoMill.Services
{
    public class Registrator
    {
        public static async Task<Facture> GetAllFactureInfosAsync(int id)
        {
            var factur = DbCon.Factures
                .Include(facture => facture.Ordonnances.Select(a => a.Medecin))
                .Include(facture => facture.Ordonnances.Select(o => o.Patient.Assure))
                .Include(facture => facture.Ordonnances.Select(o => o.Patient.Assure))
                .Include(x => x.Ordonnances.Select(a => a.Medicaments.Select(ord => ord.Medicament.Forme)))
                .FirstOrDefaultAsync(f => f.Id == id);
            return await factur;
        }
        public static DbCon DbCon => new DbCon(Settings.Default.ConnectionString);

        internal static async Task CreateDemoFactureToExcel(object caller)
        {
            var op = new OpenFolderDialog();
            if (op.ShowDialog() == true)
            {
                var controller = await DialogCoordinator.ShowProgressAsync(caller, "Transformation de la Facture en Excel ...", "S'il vous plaît, attendez");
                controller.SetProgress(0);
                try
                {
                    await Task.Delay(2000);
                    var progress = new ActionProgress<int>(i =>
                    {
                        double v = i < 100 ? i : 100;

                        double val = v / 100;

                        controller.SetProgress(val);
                        controller.SetMessage("S'il vous plaît, attendez \n       " + v + " % ");
                    });

                    var savePath = op.FolderName;
                    var facture = DbCon.Factures.FirstOrDefault();
                    if (facture != null)
                    {
                        var factur = await GetAllFactureInfosAsync(facture.Id);
                        Info pharmacieInfo = DbCon.Infos.FirstOrDefault();
                       await Task.Run(() =>  factur.ExtractFactureToExcel(savePath, pharmacieInfo, progress));
                    }
                    await controller.CloseAsync();
                }
                catch (Exception ex)
                {
                    await ShowMessage(caller, "Error", ex.Message);
                    await ex.AppLoggingAsync();
                }
                finally
                {
                    if (controller.IsOpen)
                        await controller.CloseAsync();
                }
            }
        }

        public static IDialogCoordinator DialogCoordinator { get; set; } = ViewModelLocator.Instance.DialogCoordinator;

        protected static async Task<MessageDialogResult> ShowMessage(object caller, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await DialogCoordinator.ShowMessageAsync(caller, title.GetString(), message.GetString(), style, settings ?? Statics.MessageSettings);
        }
    }
}
