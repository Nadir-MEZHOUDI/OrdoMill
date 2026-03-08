using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using OrdoMill.Views.Medicaments;
using PropertyChanged;

namespace OrdoMill.Views.Vente
{
    [AddINotifyPropertyChangedInterface]
    public sealed class OrdoViewModel : Repository<MedOrd, VentView, VentView>
    {
        public ObservableCollection<MedOrd> MedicamentsList { get; set; } = new ObservableCollection<MedOrd>();

        public OrdoViewModel()
        {

            SearchMedCommand = new RelayCommand(async () => await SaerchMedEx());
            AddMedicamentCommand = new RelayCommand(async () => await AddOrUpdateMed());
            FocusQtCommand = new RelayCommand(() => { QtFocused |= SearchedMed != null; });
            SearchedMed = null;
            IsEditable = false;
        }



        private async Task AddOrUpdateMed()
        {
            if (SearchedMed?.Id > 0)
            {
                Locator.VentMedicamentViewModel.SelectedItem = SearchedMed;
                await Locator.VentMedicamentViewModel.UpdateEx();
            }
            else
            {
                await Locator.VentMedicamentViewModel.AddEx();
            }

            new EditMedicamentView().ShowDialog();
        }

        public override async Task SearchEx()
        {
            await Task.Delay(1);
        }

        public override async Task AsyncIntialiser()
        {
            await RefreshTotal();
            await AddEx();
            await base.AsyncIntialiser();
        }

        public ObservableCollection<Medicament> AllMedicamentsList { get; set; }

        public RelayCommand SearchMedCommand { get; set; }

        public string TxtMedSearch { get; set; }

        public bool IsValidMed => SearchedMed?.Id > 0;

        public double Total { get; set; }

        public int Vignettes { get; set; }

        [AlsoNotifyFor(nameof(IsEditable))]
        public Medicament SearchedMed { get; set; }

        public SolidColorBrush VignetteColor => new SolidColorBrush((SearchedMed?.Remboursable ?? true) ? (SearchedMed?.Controle ?? false ? Colors.DodgerBlue : Colors.LimeGreen) : Colors.Red);

        public RelayCommand AddMedicamentCommand { get; set; }

        public bool QtFocused { get; set; }

        public RelayCommand FocusQtCommand { get; set; }

        public bool IsSearchMedicamentTextFocused { get; set; }

        private CancellationTokenSource searchMedcts;

        [DebuggerHidden]
        private async Task SaerchMedEx()
        {
            try
            {
                searchMedcts?.Cancel();
                searchMedcts = new CancellationTokenSource();
                var token = searchMedcts.Token;
                if (token.IsCancellationRequested) return;
                AllMedicamentsList = new ObservableCollection<Medicament>(await
                    Context.Medicaments.Where(x => (x.Nom + " " + x.Dose + " " + x.Forme.Abrg).Contains(TxtMedSearch))
                        //.Select(x => new Dto.Medicament
                        //{
                        //    Id = x.Id,
                        //    Nom = x.Nom,
                        //    Controle = x.Controle,
                        //    Remboursable = x.Remboursable,
                        //    Boite = x.Boite,
                        //    Dci = x.Dci,
                        //    Dose = x.Dose,
                        //    Forme = new Dto.Forme { Abrg = x.Forme.Abrg }
                        //})
                        .ToListAsync(token));
                SearchedMed = null;
                SearchedMed = AllMedicamentsList?.FirstOrDefault();

            }
            catch (OperationCanceledException)
            {
                // Expected when search is cancelled
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task AddEx()
        {
            SelectedItem = new MedOrd();
            IsEditable = true;
            IsFocused = true;
            await RefreshTotal();
        }

        public override async Task CancelEx()
        {
            SelectedItem = null;
            IsEditable = false;
            await Task.Delay(1);
        }

        public override async Task DeleteEx(int id = 0)
        {
            MedicamentsList?.Remove(SelectedItem);
            await RefreshTotal();
        }

        private async Task RefreshTotal()
        {
            Vignettes = MedicamentsList?.Sum(med => med.Quantite) ?? 0;
            Total = MedicamentsList?.Sum(ord => ord.Ppa * ord.Quantite) ?? 0;
            WeakReferenceMessenger.Default.Send(MedicamentsList);
            await Task.Delay(1);
        }

        public override bool SaveCanEx() => IsEditable && SearchedMed?.Id > 0 && SelectedItem?.Quantite > 0 && SelectedItem?.Ppa > 0;

        public override async Task SaveEx()
        {
            if (SelectedItem == null) return;
            SelectedItem.Medicament = Locator.Mapper.Map<Medicament>(SearchedMed);
            SelectedItem.MedicamentId = SearchedMed?.Id ?? 0;

            //ToDO Revesion
            if (!MedicamentsList.Contains(SelectedItem))
                MedicamentsList?.Add(SelectedItem);

            await RefreshTotal();
            SearchedMed = null;
            if (await ShowMessage(ViewModelLocator.Instance.VentViewModel, "Confirmation", "AddMedMsg", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                IsSearchMedicamentTextFocused = true;
                IsEditable = true;
                await AddEx();
            }
            else
            {
                IsEditable = false;
                IsFocused = false;
                IsEditable = false;
            }
        }

        public override async Task UpdateEx(int id = 0)
        {
            IsEditable = true;
            IsFocused = true;
            SearchedMed = SelectedItem.Medicament;
            await RefreshTotal();
        }
    }
}
