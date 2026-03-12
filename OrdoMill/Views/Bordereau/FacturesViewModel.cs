using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.Bordereau;

[AddINotifyPropertyChangedInterface]
public sealed class FacturesViewModel : Repository<Facture, FacturesView, FacturesView>
{
    public FacturesViewModel()
    {
        PrintFactureCommand = new RelayCommand(async () => await PrintFacturEx(), () => true);
        WeakReferenceMessenger.Default.Register<Data.Model.Info>(this, (r, msg) => PharmacieInfo = msg);
        ToBordereauCommand = new RelayCommand(ToBordereauEx);
        WeakReferenceMessenger.Default.Register<ObservableCollection<Data.Model.Bordereau>>(this, async (r, msg) => await RefreshBordereauxList());
    }

    private Data.Model.Info PharmacieInfo { get; set; }

    public Data.Model.Bordereau SelectedBordereau { get; set; }

    public RelayCommand PrintFactureCommand { get; set; }

    public ObservableCollection<Data.Model.Bordereau> Bordereaux { get; set; } =
        new ObservableCollection<Data.Model.Bordereau>();

    public RelayCommand ToBordereauCommand { get; set; }

    private async Task PrintFacturEx()
    {
        try
        {
            if (SelectedItem == null) return;

            var a = Encryptor.CompareKey(Settings.Default.Key);
            if (Debugger.IsAttached || a)
            {
                await ConvertFacturToExcel();
            }
            else
            {
                //await Locator.Main.LicenseDialog.ShowDialogIfNecessaries();
                //var result = Locator.Main.LicenseDialog.CheckShowDialogesConditions();
               // if (result)
                {
                 //   await Registrator.CreateDemoFactureToExcel(this);
                }
              //  else
                {
                    await ConvertFacturToExcel();
                }
            }
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }

    private void ToBordereauEx()
    {
        try
        {
            foreach (var facture in ListToDisplay.Where(x => x.IsCheck))
                if (facture.Bordereau == null)
                    facture.Bordereau = SelectedBordereau;
        }
        catch (Exception ex)
        {
            ex.AppLogging();
        }
    }

    public override async Task AsyncIntialiser()
    {
        try
        {
            await RefreshBordereauxList();
            await GetPharmacieInfo();
            await base.AsyncIntialiser();
            await ExtractionMethods.CheckTemplates();

        }
        catch (Exception ex)
        {
            ex.AppLogging();
        }
    }

    public override async Task SaveEx()
    {
        await base.SaveEx();
        WeakReferenceMessenger.Default.Send(new ObservableCollection<Facture>());
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

    private async Task ConvertFacturToExcel()
    {
        var op = new OpenFolderDialog();
        if ((op.ShowDialog() == true) && (SelectedItem != null))
        {
            var controller =
                await ShowProgressMessage("Transformation de la Facture en Excel ...", "S'il vous pla�t, attendez");
            controller.SetProgress(0);
            try
            {
                if (!await GetPharmacieInfo())
                {
                    controller.SetTitle("Error");
                    controller.SetMessage("Pharmacie info Null");
                    await Task.Delay(1000);
                    await controller.CloseAsync();
                    return;
                }
                await Task.Delay(1000);
                var progress = new ActionProgress<int>(i =>
                {
                    double v = i < 100 ? i : 100;

                    var val = v / 100;

                    controller.SetProgress(val);
                    controller.SetMessage("S'il vous pla�t, attendez \n" + v + " % ");
                });

                var savePath = op.FolderName;
                var factur = await FacturesAndBordereauService.GetAllFactureInfosAsync(SelectedItem.Id);
                await Task.Run(() => factur.ExtractFactureToExcel(savePath, PharmacieInfo, progress));

                await controller.CloseAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorMessage(ex.Message);
                await ex.AppLoggingAsync();
            }
            finally
            {
                if (controller.IsOpen)
                    await controller.CloseAsync();
            }
        }
    }

    private async Task RefreshBordereauxList()
    {
        try
        {
            var query =
                await Context.Bordereaus.Include(x => x.Factures).OrderByDescending(f => f.Number).ToListAsync();
            Bordereaux = new ObservableCollection<Data.Model.Bordereau>();
            foreach (var b in query)
                Bordereaux.Add(b);
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
                SearchExpression = facture => true;
            else
                SearchExpression = facture => facture.Number.ToString().Contains(SearchPattern);

            ItemsList = Context.Factures
                .Where(SearchExpression)
                .OrderByDescending(x => x.Number)
                .Select(x => new Dto.Facture
                {
                    OrdonancesCount = x.Ordonnances.Count,
                    Montant = x.Ordonnances.Sum(o => o.Medicaments.Sum(m => m.Ppa * m.Quantite)),
                    Id = x.Id,
                    Chronic = x.Chronic,
                    Out = x.Out,
                    BordereauId = x.BordereauId,
                    Number = x.Number,
                    Bordereau = x.Bordereau
                });
            if (SelectedBordereau != null)
                ItemsList = ItemsList.Where(facture => facture.BordereauId == SelectedBordereau.Id);

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
            SelectedItem.Number = (await Context.Factures.MaxAsync<Facture, int?>(x => x.Number) ?? 0) + 1;
            var lastId = await Context.Bordereaus.MaxAsync<Data.Model.Bordereau, int?>(x => x.Number) ?? 0;
            var b = Context?.Bordereaus?.FirstOrDefault(x => x.Number == lastId);
            if (b != null)
                SelectedItem.Bordereau = b;
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }
}
