using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using OrdoMill.Views.Clients;
using OrdoMill.Views.Ordonnance;
using OrdoMill.Views.Patients;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Vente
{
    [ImplementPropertyChanged]
    public sealed class VentViewModel : Repository<Data.Model.Ordonnance, VentView, VentView>
    {
        public VentViewModel()
        {
            if (IsInDesignMode)
            {
                Matricule = "123456789";
                Assure = new Assure
                {
                    Matricule = "1234567890",
                    Grade = "مستخدم مدني",
                    Nom = "Mezhoudi",
                    Prenom = "Hadj Nadir",
                    ArNom = "مزهودي",
                    ArPrenom = "الحاج النذير",
                    Tel = "0666346066"
                };
            }
            else
            {
                SelectedItem = new Data.Model.Ordonnance();
                OrdoViewModel = new OrdoViewModel();
                SearchPathologieCommand = new RelayCommand(async () => await SearchPathologieEx());
                ShowPreviewPopupCommand = new RelayCommand<Data.Model.Ordonnance>(ShowPreviewEx);
                AddOrUpdateClientCommand = new RelayCommand(async () => await AddOrUpdateClientEx());
                AddOrUpdatePatientCommand = new RelayCommand(async () => await AddOrUpdatePatientEx());
                SearchMedecinsCommand = new RelayCommand(async () => await SearchMedecinsEx());
                SearchClient = new RelayCommand(async () => await SearchClientEx(Matricule));
                CheckPatient = new RelayCommand(async () => await CheckPatientEx());
                Messenger.Default.Register<Data.Model.Ordonnance>(this, msg => { SelectedItem = msg; });
                Messenger.Default.Register<ObservableCollection<MedOrd>>(this, msg =>
                {
                    if (SelectedItem != null)
                    {
                        SelectedItem.Medicaments = null;
                        SelectedItem.Medicaments = msg;
                    }
                });
                Messenger.Default.Register<ObservableCollection<Pathologie>>(this, msg => Pathologies = msg);
                IsEditable = true;
                IsFocused = true;
            }
        }

        public RelayCommand<Data.Model.Ordonnance> ShowPreviewPopupCommand { get; set; }


        public OrdoViewModel OrdoViewModel { get; set; }

        public RelayCommand CheckPatient { get; set; }
        private ObservableCollection<Pathologie> Pathologies { get; set; }
        public bool IsValidClient => (Assure != null) && !Assure.Suspende;
        public bool IsValidPatient { get; set; }
        public Assure Assure { get; set; }
        public int SelectedSpecialiteId { get; set; }
        public string NomDeMedecin { get; set; }
        public ObservableCollection<Medecin> Medecins { get; set; }
        public string Matricule { get; set; }
        public RelayCommand SearchClient { get; set; }

        public SolidColorBrush NameColor
            => new SolidColorBrush((!Assure?.Suspende ?? false) ? Colors.Green : Colors.Red);

        public RelayCommand SearchMedecinsCommand { get; set; }
        public RelayCommand AddOrUpdateClientCommand { get; set; }
        public string Pathologie { get; set; }
        public RelayCommand SearchPathologieCommand { get; set; }
        public RelayCommand AddOrUpdatePatientCommand { get; set; }

        public SolidColorBrush IsValidClientBackground
            =>
            new SolidColorBrush(Assure == null ? Colors.White : Assure.Suspende ? Colors.OrangeRed : Colors.LimeGreen);

        public string FactureOrder => $"{SelectedFacture?.Ordonnances?.Count + 1}/ 50";
        public ObservableCollection<Facture> FacturesList { get; set; }
        public Facture SelectedFacture { get; set; }
        private void ShowPreviewEx(Data.Model.Ordonnance ordonnance) => new OrdonancePreview(ordonnance).ShowDialog();

        // public string FactureOrder => Context.Ordonnances.Count(x =>  SelectedFacture != null && x.FactureId == SelectedFacture.Id) + " / 50";

        public override async Task CancelEx()
        {
            await base.CancelEx();
            await AddEx();
        }

        public override bool AddCanEx() => true;

        public override async Task AddEx()
        {
            Matricule = "";
            Assure = null;
            IsFocused = true;
            NomDeMedecin = "";
            IsValidPatient = false;
            SelectedSpecialiteId = -1;
            SelectedItem = null;
            SelectedItem = new Data.Model.Ordonnance();
            OrdoViewModel = null;
            OrdoViewModel = new OrdoViewModel();
            await base.AddEx();
            await OrdoViewModel.AddEx();
        }

        public override bool SaveCanEx()
            =>
            IsEditable && (SelectedItem != null) && (SelectedItem?.Medicaments?.Count > 0) &&
            NomDeMedecin.IsNotNullOrEmpty();

        public override async Task SaveEx()
        {
            try
            {
                var result = await ShowMessage("Confirmation", "SignerMsg", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    SelectedItem.MedecinId = await GetMedecinByNameAndSpecialite();
                    SelectedItem.FactureId = SelectedFacture?.Id;
                    SelectedItem.Medicaments = new List<MedOrd>(OrdoViewModel.MedicamentsList);
                    await base.SaveEx();
                    await AddEx();
                }
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task SearchEx() => await Task.Delay(100);

        private async Task<int> GetMedecinByNameAndSpecialite()
        {
            try
            {
                var medecin = new Medecin { Nom = NomDeMedecin, Type = SelectedSpecialiteId };
                if (!await Context.Medecins.AnyAsync(x => (x.Nom == NomDeMedecin) && (x.Type == SelectedSpecialiteId)))
                {
                    Context.Medecins.Add(medecin);
                    Context.Medecins.Add(medecin);
                    await Context.SaveChangesAsync();
                }
                return (await Context.Medecins.FirstOrDefaultAsync(x => (x.Nom == NomDeMedecin) && (x.Type == SelectedSpecialiteId)))?.Id ?? 0;

            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
                return 0;
            }
        }

        public override async Task AsyncIntialiser()
        {
            SelectedSpecialiteId = 0;
            NomDeMedecin = "";
            Pathologies = new ObservableCollection<Pathologie>(await Context.Pathologies
                //.Select(x => new Dto.Pathologie
                //{
                //    Code = x.Code,
                //    Nom = x.Nom,
                //    Id = x.Id
                //})
                .ToListAsync());

            Medecins =
                new ObservableCollection<Medecin>(
                    await
                        Context.Medecins
                            //.Select(x => new Dto.Medecin
                            //{
                            //    Id = x.Id,
                            //    Nom = x.Nom,
                            //    Type = x.Type
                            //})
                            .ToListAsync());

            await RefreshFacturesList();
            Messenger.Default.Register<ObservableCollection<Facture>>(this, async msg => await RefreshFacturesList());
        }

        private async Task AddOrUpdatePatientEx()
        {
            if (SelectedItem?.Patient != null)
                await Locator.VentPatientsViewModel.UpdateEx();
            else
                await Locator.VentPatientsViewModel.ShowMeAddEx(Assure);
            new EditPatientView().ShowDialog();
        }

        private async Task SearchPathologieEx()
        {
            try
            {
                Pathologie = Pathologies?.FirstOrDefault(x => x.Code == SelectedItem?.PathologieCode)?.Nom;

            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        private async Task AddOrUpdateClientEx()
        {
            try
            {
                if (Assure?.Id > 0)
                {
                    Locator.VentClientsViewModel.SelectedItem = Assure;
                    await Locator.VentClientsViewModel.UpdateEx();
                }
                else
                {
                    await Locator.VentClientsViewModel.AddEx(Matricule);
                }
            }
            catch (Exception ex)
            {

                await ex.AppLoggingAsync();
            }

            new EditClientView().ShowDialog();
        }

        private async Task SearchMedecinsEx()
        {
            NomDeMedecin = "";
            Medecins = new ObservableCollection<Medecin>(await Context.Medecins
                .Where(x => x.Type == SelectedSpecialiteId)
                //.Select(x => new Dto.Medecin
                //{
                //    Nom = x.Nom,
                //    Id = x.Id,
                //    Type = x.Type
                //})

                .ToListAsync());
        }

        private async Task CheckPatientEx()
        {

            var p = await Context.Patients.FirstOrDefaultAsync(x => x.Id == SelectedItem.Patient.Id);

            IsValidPatient = !p?.Suspende ?? false;

            //if (p?.DateNaissance?.Subtract(DateTime.Now).TotalDays < 18 * 365.4)
            //{
            //    IsValidPatient = true;
            //    return;
            //}
            //if (p?.Au == null)
            //{
            //    IsValidPatient = true;
            //    return;
            //}
            //if (p.Au.Value.CompareTo(DateTime.Now) <= 0)
            //{
            //    IsValidPatient = !p.Assure.Suspende;
            //    await ShowMessage("Avertissement", "DureeMsg");
            //}
            //else if (p.Au.Value.CompareTo(DateTime.Now) > 0)
            //{
            //    IsValidPatient = !p.Assure.Suspende;
            //    OrdoViewModel.AddCommand.Execute(null);
            //}
        }


        private CancellationTokenSource searchClientCts;
        [DebuggerHidden]
        public async Task SearchClientEx(string Matricule)
        {
            try
            {
                Assure = null;
                searchClientCts?.Cancel();
                searchClientCts = new CancellationTokenSource();
                var token = searchClientCts.Token;
                await Task.Delay(100, token);
                if (token.IsCancellationRequested) return;
                Assure = await Context.Assures
                    .Select(x => new Dto.Assure
                    {
                        Matricule = x.Matricule,
                        Nom = x.Nom,
                        Prenom = x.Prenom,
                        DateNaissance = x.DateNaissance,
                        Id = x.Id,
                        Suspende = x.Suspende,
                        Patients = x.Patients
                    })
                    .FirstOrDefaultAsync(x => x.Matricule == Matricule, token);

            }
            catch (OperationCanceledException c)
            {

            }
            catch (Exception ex)
            {

                // await ex.AppLoggingAsync();
            }
        }

        public override async Task UpdateEx(int id = 0)
        {
            try
            {
                var ordonnance = await Context.Ordonnances
                   .Include(o => o.Patient)
                   .Include(o => o.Patient.Assure)
                   .Include(o => o.Medecin)
                   .FirstOrDefaultAsync(x => x.Id == SelectedFacture.Id);

                if (ordonnance != null)
                {
                    OrdoViewModel = new OrdoViewModel();
                    SelectedItem = ordonnance;
                    Matricule = ordonnance.Patient?.Assure?.Matricule;
                    Assure = ordonnance.Patient?.Assure;
                    SelectedSpecialiteId = ordonnance.Medecin?.Type ?? 0;
                    SelectedItem.Patient = ordonnance.Patient;
                    SelectedItem.PatientId = ordonnance.PatientId;
                    SelectedItem.Medecin = ordonnance.Medecin;
                    SelectedItem.Facture = ordonnance.Facture;
                    SelectedFacture = FacturesList.FirstOrDefault(x => x.Id == ordonnance?.FactureId);
                    NomDeMedecin = ordonnance.Medecin?.Nom;
                    OrdoViewModel.MedicamentsList = new ObservableCollection<MedOrd>(ordonnance.Medicaments);
                    IsEditable = true;
                    IsFocused = true;
                    await CheckPatientEx();
                }
            }
            catch (Exception ex)
            {

                await ex.AppLoggingAsync();
            }
        }

        private async Task RefreshFacturesList()
        {
            try
            {
                FacturesList = new ObservableCollection<Facture> { new Facture { Number = 0 } };
                var query = await Context.Factures
                            //.Select(x => new Dto.Facture
                            //{
                            //    Id = x.Id,
                            //    Number = x.Number,
                            //    Chronic = x.Chronic,
                            //    Out = x.Out
                            //})
                            .OrderByDescending(f => f.Number)
                            .ToListAsync();
                query.ForEach(f => FacturesList.Add(f));
                //foreach (var facture in query)
                //    FacturesList.Add(facture);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
    }
}