using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using OrdoMill.Views.Vente;
using PropertyChanged;
using SmartApp.Helpers.Helpers;
using Assure = OrdoMill.Dto.Assure;
using Patient = OrdoMill.Dto.Patient;

namespace OrdoMill.Views.Ordonnance
{
    [AddINotifyPropertyChangedInterface]
    public class OrdonnanceViewModel : Repository<Data.Model.Ordonnance, ShowOrdonancesView, ShowOrdonancesView>
    {
        public OrdonnanceViewModel()
        {
            try
            {
                PageSize = 50;
                RefreshCommand = new RelayCommand(async () => await RefreshEx());
                CheckCommand = new RelayCommand(CheckErrorsEx);
                CocherTousCommand = new RelayCommand(CocherTousEx);
                DecocherTousCommand = new RelayCommand(DeCocherTousEx);
                ClôturerCommand = new RelayCommand(async () => await ClotureEx());
                ShowDetailsCommand = new RelayCommand<Data.Model.Ordonnance>(o => new OrdonancePreview(o).ShowDialog(),
                    o => true);
                var initial = AsyncIntialiser();
            }
            catch (Exception)
            {
            }
        }

        public ObservableCollection<Facture> FacturesList { get; set; }
        public Facture SelectedFacture { get; set; }

        private double ExtractionProgress { get; set; }

        public RelayCommand RefreshCommand { get; set; }

        public RelayCommand DecocherTousCommand { get; set; }

        public string StatusText { get; set; }

        public RelayCommand ClôturerCommand { get; set; }

        public RelayCommand CheckCommand { get; set; }


        public RelayCommand CocherTousCommand { get; private set; }

        public RelayCommand<Data.Model.Ordonnance> ShowDetailsCommand { get; set; }

        private async Task RefreshFacturesList()
        {
            try
            {
                FacturesList = new ObservableCollection<Facture> { new Facture { Number = 0 } };
                var query = await Context.Factures.OrderByDescending(f => f.Number).ToListAsync();
                foreach (var facture in query)
                    FacturesList.Add(facture);
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        private async Task<int> GetLastFacturNumbers()
            => (await Context.Factures?.OrderByDescending(f => f.Id)?.FirstOrDefaultAsync())?.Number ?? 0;

        private void CocherTousEx()
        {
            foreach (var o in ListToDisplay)
                o.IsCheck = true;
            //RaisePropertyChanged(nameof(ListToDisplay));
            // OnPropertyChanged(nameof(ListToDisplay));
            // ListToDisplay = new ObservableCollection<Ordonnance>(ListToDisplay);
        }

        private void DeCocherTousEx()
        {
            // ListToDisplay.AsParallel()
            foreach (var o in ListToDisplay)
                o.IsCheck = false;
            // RaisePropertyChanged(nameof(ListToDisplay));
            //ListToDisplay = new ObservableCollection<Ordonnance>(ListToDisplay);
            // OnPropertyChanged(nameof(ListToDisplay));
        }

        private async Task ExtractFactures(ProgressDialogController controller, bool chronic, bool? outRegion = null)
        {
            try
            {
                var selectedOrdonnances =
                    ListToDisplay.Where(
                            x =>
                                x.IsCheck && (x.IsChronique == chronic) &&
                                ((outRegion == null) || (x.IsHorsRegion == outRegion)))
                        .ToList();
                var facturesNumber = selectedOrdonnances.Count / 50;

                if ((selectedOrdonnances.Count > 0) && (selectedOrdonnances.Count % 50 != 0))
                    facturesNumber += 1;

                controller.SetProgress(GetProgressValue());
                for (var i = 0; i < facturesNumber; i++)
                {
                    controller.SetProgress(GetProgressValue());
                    if (controller.IsCanceled) await controller.CloseAsync();
                    var tempList = selectedOrdonnances.Skip(50 * i).Take(50).ToList();
                    var facture = new Facture
                    {
                        Ordonnances = new ObservableCollection<Data.Model.Ordonnance>(tempList),
                        Number = await GetLastFacturNumbers() + 1,
                        Chronic = chronic,
                        Out = outRegion ?? false
                    };
                    Context.Factures.Add(facture);
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ex.AppLogging();

            }
        }

        private double GetProgressValue()
        {
            ExtractionProgress++;
            var a = ExtractionProgress / ItemsList.AsEnumerable().Count(x => x.IsCheck) < 1
                ? ExtractionProgress / ItemsList.AsEnumerable().Count(x => x.IsCheck)
                : 1;
            return a;
        }

        private void CheckErrorsEx()
        {
            try
            {
                StatusText = "..........";
                var errorsCount = 0;

                foreach (var ord in ListToDisplay)
                {
                    var ordoProblem = "";
                    if ((ord.Patient?.Prenom == null) || ord.Patient.Prenom.IsNullOrEmpty())
                        ordoProblem += "Manque Prénom de patient\n";

                    if ((ord.Patient?.Nom == null) || ord.Patient.Nom.IsNullOrEmpty())
                        ordoProblem += "Manque Nom de patient\n";

                    if (ord.Patient?.DateNaissance == null)
                        ordoProblem += "Manque DateNaissance de patient\n";

                    if ((ord.Patient?.Assure?.IsContractial == true) && (ord.Patient?.Assure?.Jusquea == null))
                        ordoProblem += "Manque Fin de Contrat d'Assuré\n";

                    if ((ord.Patient?.Au == null) && (ord?.Patient?.Nom != ord?.Patient?.Assure?.Nom) &&
                        (ord?.Patient?.Prenom != ord?.Patient?.Assure?.Prenom))
                        ordoProblem += "Manque Nom de patient\n";

                    if ((ord.Patient?.Assure?.Prenom == null) || ord.Patient.Assure.Prenom.IsNullOrEmpty())
                        ordoProblem += "Manque Prénom d'Assuré\n";

                    if ((ord.Patient?.Assure?.Nom == null) || ord.Patient.Assure.Nom.IsNullOrEmpty())
                        ordoProblem += "Manque Nom d'Assuré\n";

                    if ((ord.Patient?.Assure?.Adresse == null) || ord.Patient.Assure.Adresse.IsNullOrEmpty())
                        ordoProblem += "Manque Adresse d'Assuré\n";

                    if (ord.Patient?.Assure?.DateNaissance == null)
                        ordoProblem += "Manque DateNaissance d'Assuré\n";

                    if ((ord.Medecin?.Nom == null) || ord.Medecin.Nom.IsNullOrEmpty())
                        ordoProblem += "Manque Nom de Médecin\n";

                    ord.Problems = new List<string>(ordoProblem.Split('\n'));

                    if (ordoProblem.IsNotNullOrEmpty()) errorsCount++;

                    ord.IsCheck = ordoProblem.IsNullOrEmpty();

                    StatusText = $"Ordonnances: {ItemsList.Count()}  /  " +
                                 $"Normal: {ItemsList.Count(x => x.IsChronique == false)}  /  " +
                                 $"Chronique: {ItemsList.AsEnumerable().Count(x => x.IsChronique && (x.IsHorsRegion == false))}  /  " +
                                 $"HR: {ItemsList.AsEnumerable().Count(x => x.IsChronique && x.IsHorsRegion)}  /  " +
                                 $"Erreurs: {errorsCount}";
                }
            }
            catch (Exception ex)
            {
                ex.AppLogging();

            }
        }

        private async Task RefreshEx()
        {
            try
            {
                StatusText = "..........";

                var controller = await ShowProgressMessage("Working", "Wait");
                controller.SetIndeterminate();

                await Task.Delay(1000);

                ItemsList = Context.Ordonnances.OrderBy(x => x.Id)
                        //.Include(p => p.Medicaments)
                        //.Include(p => p.Patient.Assure)
                        //.Include(x => x.Medecin)
                        .Where(x => x.FactureId == null)
                        .Select(x => new Dto.Ordonnance
                        {
                            Id = x.Id,
                            Medicaments = x.Medicaments,
                            Patient = new Patient
                            {
                                Assure = new Assure
                                {
                                    Id = x.Patient.AssureId,
                                    Nom = x.Patient.Assure.Nom,
                                    Prenom = x.Patient.Assure.Prenom,
                                    Matricule = x.Patient.Assure.Matricule
                                },
                                Nom = x.Patient.Nom,
                                Prenom = x.Patient.Prenom,
                                Id = x.PatientId
                            },
                            Medecin = x.Medecin,
                            Montant = x.Medicaments.Sum(m => m.Ppa * m.Quantite)
                        })
                    ;

                await BaseSearchEx();

                await controller.CloseAsync();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }


        private async Task ClotureEx()
        {
            var controller = await ShowProgressMessage("Working", "Please Wait");
            controller.SetProgress(0);

            try
            {
                await ExtractFactures(controller, false);

                await ExtractFactures(controller, true);

                await ExtractFactures(controller, true, true);

                await RefreshEx();
            }
            catch (Exception ex)
            {
                await ShowErrorMessage("حدث خطأ أثناء الاستخراج قم باتحقق من البيانات ثم أعد المحاولة");

                await ex.AppLoggingAsync();
            }
            finally
            {
                if (controller.IsOpen)
                    await controller.CloseAsync();
            }
        }

        public override async Task AsyncIntialiser()
        {
            try
            {
                await NavigateTo(typeof(ShowOrdonancesView));
                await GetLastFacturNumbers();
                await RefreshFacturesList();
                await base.AsyncIntialiser();
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        public override async Task AddEx()
        {
            try
            {
                await Locator.VentViewModel.AddEx();
                await ViewModelLocator.Instance.StaticServices.NavigationEx(typeof(VentView));
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        public override async Task UpdateEx(int id = 0)
        {
            try

            {
                await ViewModelLocator.Instance.VentViewModel.UpdateEx(id);
                await ViewModelLocator.Instance.StaticServices.NavigationEx(typeof(VentView));
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        public override async Task DeleteEx(int id = 0)
        {
            try
            {
                var result = await ShowMessage("Confirmation", "SupprimeMsg", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    var query = ItemsList.AsEnumerable().Where(o => o.IsCheck).ToList();
                    foreach (var ordo in query)
                    {
                        Context.MedOrds.RemoveRange(ordo.Medicaments);
                        Context.Entry(ordo).State = EntityState.Deleted;
                        await Context.SaveChangesAsync();
                    }
                }
                await SearchEx();
            }
            catch (Exception ex)
            {
                await ShowErrorMessage("DeleteErrorMgs");
                await ex.AppLoggingAsync();
            }
        }


        public override async Task SearchEx()
        {
            try
            {
                if (SearchPattern.IsNullOrEmpty())
                    SearchExpression = ordonnance => true;
                else if (SearchPattern.IsNumeric())
                    SearchExpression = ord => ord.Patient.Assure.Matricule.Contains(SearchPattern);
                else
                    SearchExpression = ord => (ord.Patient.Nom + " " + ord.Patient.Prenom).Contains(SearchPattern) ||
                                              (ord.Patient.Assure.Nom + " " + ord.Patient.Assure.Prenom).Contains(
                                                  SearchPattern);


                ItemsList = Context.Ordonnances
                        //                               .Include(x => x.Medecin)
                        //                               .Include(x => x.Patient)
                        //                               .Include(x => x.Facture)
                        //                               .Include(x => x.Patient.Assure)
                        //                               .Include(x => x.Medicaments.Select(ord => ord.Medicament))
                      
                        .Where(SearchExpression)
                        .OrderBy(x => x.Id)
                    .Select(o => new Dto.Ordonnance
                    {
                        Id = o.PatientId,
                        SoineDate = o.SoineDate,
                        ServiDate = o.ServiDate,
                        FactureId = o.FactureId,
                        IsChronique = o.IsChronique,
                        IsHorsRegion = o.IsHorsRegion,
                        Patient = new Patient
                        {
                            Id = o.PatientId,
                            Nom = o.Patient.Nom,
                            Prenom = o.Patient.Prenom,
                            Assure = new Assure
                            {
                                Matricule = o.Patient.Assure.Matricule,
                                Id = o.Patient.AssureId,
                                Nom = o.Patient.Assure.Nom,
                                Prenom = o.Patient.Assure.Prenom
                            }
                        }
                    })
                    ;
                if ((SelectedFacture != null) && (SelectedFacture?.Id == 0))
                    ItemsList = ItemsList.Where(ordonnance => ordonnance.Facture == null);
                else if (SelectedFacture != null)
                    ItemsList = ItemsList.Where(ordonnance => ordonnance.FactureId == SelectedFacture.Id);

                StatusText = $"Facture {SelectedFacture?.MinName} / Ordonnances: {ItemsList?.Count()}";
                await BaseSearchEx();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }
    }
}
