using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OrdoMill.Services;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Bordereau
{
    [AddINotifyPropertyChangedInterface]
    public   class BordereauxViewModel : Repository<Data.Model.Bordereau, BordereauView, BordereauView>
    {
        public BordereauxViewModel()
        {
            PrintFacturesCommand = new RelayCommand(async () => await PrintFactursEx(), CanPrint);
            PrintResumeCommand = new RelayCommand(async () => await PrintResumeEx(), CanPrint);
            PrintEtiquettesCommand = new RelayCommand(async () => await PrintEtiquettesEx(), CanPrint);
            Messenger.Default.Register<Data.Model.Info>(this, msg => PharmacieInfo = msg);
        }

        private Data.Model.Info PharmacieInfo { get; set; }

        public RelayCommand PrintFacturesCommand { get; set; }
        public RelayCommand PrintResumeCommand { get; set; }

        public RelayCommand PrintEtiquettesCommand { get; set; }


        public override async Task SaveEx()
        {
            await base.SaveEx();
            Messenger.Default.Send(new ObservableCollection<Data.Model.Bordereau>());
        }

        private async Task PrintResumeEx()
        {
            try
            {
                var op = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };

                if ((op.ShowDialog() == DialogResult.OK) && (SelectedItem != null))
                {
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    var savePath = op.SelectedPath;
                    await bordereau.ExtractBordereauResume(savePath, PharmacieInfo);
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        private async Task PrintEtiquettesEx()
        {
            try
            {
                var op = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                if ((op.ShowDialog() == DialogResult.OK) && await GetPharmacieInfo() && (SelectedItem != null))
                {
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    var savePath = op.SelectedPath;
                    await bordereau.ExtractBordereauEtiquette(savePath, PharmacieInfo);
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task AsyncIntialiser()
        {
            await GetPharmacieInfo();
            await base.AsyncIntialiser();
        }

        private async Task<bool> GetPharmacieInfo()
        {
            try
            {
                PharmacieInfo = await Context.Infos.FirstOrDefaultAsync();
                return PharmacieInfo != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CanPrint() => SelectedItem != null;

        private async Task PrintFactursEx()
        {
            try
            {
                var op = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    RootFolder = Environment.SpecialFolder.Desktop
                };
                if ((op.ShowDialog() == DialogResult.OK) && await GetPharmacieInfo() && (SelectedItem != null))
                {
                    var controller = await ShowProgressMessage("Extraction", "...");
                    controller.SetIndeterminate();
                    await Task.Delay(1000);
                    var savePath = op.SelectedPath;
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    //await Task.Run(() =>  
                    bordereau.ExtractAllBordereauFactures(savePath, PharmacieInfo);
                    //);
                    await controller.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task SearchEx()
        {
            try
            {
                if (SearchPattern.IsNullOrEmpty())
                    SearchExpression = bordereau => true;
                else SearchExpression = medecin => medecin.Number.ToString().Contains(SearchPattern);
                ItemsList = Context.Bordereaus
                        // .Include(f => f.Factures.Select(a => a.Ordonnances.Select(ordonnance => ordonnance.Medicaments)))
                        .Where(SearchExpression)
                        .OrderBy(bordereau => bordereau.Number)
                        .Select(x => new Dto.Bordereau
                        {
                            Date = x.Date,
                            Montant = x.Factures.Sum(f => f.Ordonnances.Sum(o => o.Medicaments.Sum(med => med.Quantite * med.Ppa))),
                            Number = x.Number,
                            Id = x.Id
                        })
                    ;

                await BaseSearchEx();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task AddEx()
        {
            try
            {
                await base.AddEx();
                SelectedItem.Number = (Context?.Bordereaus.Max<Data.Model.Bordereau, int?>(x => x.Number) ?? 0) + 1;
                SelectedItem.Date = Context?.Bordereaus.Max(x => x.Date)?.AddMonths(1) ?? DateTime.Now;
            }

            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
    }
}