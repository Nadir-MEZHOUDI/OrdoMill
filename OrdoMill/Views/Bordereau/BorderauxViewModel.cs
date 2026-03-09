using System;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

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
            WeakReferenceMessenger.Default.Register<Data.Model.Info>(this, (r, msg) => PharmacieInfo = msg);
        }

        private Data.Model.Info PharmacieInfo { get; set; }

        public RelayCommand PrintFacturesCommand { get; set; }
        public RelayCommand PrintResumeCommand { get; set; }

        public RelayCommand PrintEtiquettesCommand { get; set; }


        public override async Task SaveEx()
        {
            await base.SaveEx();
            WeakReferenceMessenger.Default.Send(new ObservableCollection<Data.Model.Bordereau>());
        }

        private async Task PrintResumeEx()
        {
            try
            {
                var op = new OpenFolderDialog();

                if ((op.ShowDialog() == true) && (SelectedItem != null))
                {
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    var savePath = op.FolderName;
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
                var op = new OpenFolderDialog();
                if ((op.ShowDialog() == true) && await GetPharmacieInfo() && (SelectedItem != null))
                {
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    var savePath = op.FolderName;
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
                var op = new OpenFolderDialog();
                if ((op.ShowDialog() == true) && await GetPharmacieInfo() && (SelectedItem != null))
                {
                    var controller = await ShowProgressMessage("Extraction", "...");
                    controller.SetIndeterminate();
                    await Task.Delay(1000);
                    var savePath = op.FolderName;
                    var bordereau = await FacturesAndBordereauService.GetFullBordereauInfo(SelectedItem.Id);
                    //await Task.Run(() =>  
                    await bordereau.ExtractAllBordereauFactures(savePath, PharmacieInfo);
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
