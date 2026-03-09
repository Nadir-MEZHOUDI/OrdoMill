using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.Patients
{
    [AddINotifyPropertyChangedInterface]
    public sealed class PatientsViewModel : Repository<Patient, ShowPatientsView, PatientDetailsView>
    {
        public PatientsViewModel()
        {
            SelectedAssure = new Assure();
            ShowMeAndAddCommand = new RelayCommand<Assure>(async a => await ShowMeAddEx(a));
            WeakReferenceMessenger.Default.Register<Assure>(this, (r, msg) => SelectedAssure = msg);
            WeakReferenceMessenger.Default.Register<ObservableCollection<Assure>>(this, (r, msg) => AssuresList = msg);
            SuspendCommand = new RelayCommand(async () => await SuspendEx(true));
            UnSuspendCommand = new RelayCommand(async () => await SuspendEx(false));
            ChangePatientNameCommand = new RelayCommand(() =>
            {
                if (SelectedItem?.Nom != null && SelectedItem.Alliance < 2) SelectedItem.Nom = SelectedAssure?.Nom;
            });
        }

        public RelayCommand SuspendCommand { get; set; }
        public RelayCommand UnSuspendCommand { get; set; }

        public RelayCommand<Assure> ShowMeAndAddCommand { get; set; }

        public ObservableCollection<string> Alliances => new ObservableCollection<string>("Alliances".GetArray());

        public Assure SelectedAssure { get; set; }

        public ObservableCollection<Assure> AssuresList { get; set; }

        public RelayCommand ChangePatientNameCommand { get; set; }

        private async Task SuspendEx(bool value)
        {
            try
            {
                foreach (var assure in ListToDisplay.Where(x => x.IsCheck))
                {
                    assure.Suspende = value;
                    Context.Patients.AddOrUpdate(assure);
                }
                await Context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        //private void SelectePatientEx(Patient patient)
        //{
        //    SelectedItem = patient;
        //}

        public override async Task AsyncIntialiser()
        {
            try
            {
                await NavigateTo(typeof(ShowPatientsView));
                AssuresList = new ObservableCollection<Assure>(await
                            Context.Assures
                                .Select(x => new Dto.Assure
                                {
                                    Id = x.Id,
                                    Matricule = x.Matricule,
                                    Nom = x.Nom,
                                    Prenom = x.Prenom
                                })
                                .ToListAsync());
                await SearchEx();

            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public async Task ShowMeAddEx(Assure obj)
        {
            try
            {
                SelectedAssure = obj;
                await AddEx();

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
                SelectedItem = null;

                if (SearchPattern.IsNullOrEmpty())
                    SearchExpression = a => true;

                else if (SearchPattern.IsNumeric())
                    SearchExpression = p => p.Assure.Matricule.Contains(SearchPattern);

                else
                    SearchExpression = p => (p.Nom + " " + p.Prenom).Contains(SearchPattern) ||
                                            (p.Prenom + " " + p.Nom).Contains(SearchPattern);

                ItemsList = Context.Patients.Where(SearchExpression)
                    .OrderBy(x => x.Assure.Matricule)
                    .Select(x => new Dto.Patient
                    {
                        Id = x.Id,
                        Nom = x.Nom,
                        Prenom = x.Prenom,
                        Suspende = x.Suspende,
                        AssureId = x.AssureId,
                        Assure = new Dto.Assure
                        {
                            Id = x.AssureId,
                            Matricule = x.Assure.Matricule,
                            Nom = x.Nom,
                            Prenom = x.Prenom,
                        }
                    })
                    ;
                await BaseSearchEx();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task SaveEx()
        {
            try
            {
                SelectedItem.Assure = null;
                SelectedItem.AssureId = SelectedAssure?.Id ?? 0;
                await base.SaveEx();
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
                SelectedItem = new Patient { AssureId = SelectedAssure?.Id ?? 0 };
                IsEditable = true;
                IsFocused = true;
                await NavigateTo(typeof(PatientDetailsView));

            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task UpdateEx(int id = 0)
        {
            try
            {
                await base.UpdateEx(id);
                SelectedAssure = AssuresList.FirstOrDefault(x => x.Id == SelectedItem?.AssureId);
                //if (SelectedItem != null) SelectedItem.AssureId = SelectedAssure?.Id ?? 0;
                await NavigateTo(typeof(PatientDetailsView));
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public override async Task DeleteEx(int id = 0)
        {
            try
            {
                foreach (var ordonnance in SelectedItem.Ordonnances)
                {
                    Context.MedOrds.RemoveRange(ordonnance.Medicaments);
                    Context.Ordonnances.Remove(ordonnance);
                }
                await base.DeleteEx(id);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                await ex.AppLoggingAsync();
            }
        }
    }
}