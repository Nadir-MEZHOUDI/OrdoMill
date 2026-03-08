using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Clients
{
    [ImplementPropertyChanged]
    public sealed class ClientsViewModel : Repository<Assure, ShowClientsView, ClientDetailsView>
    {
        public ClientsViewModel()
        {
            SuspendCommand = new RelayCommand(async () => await SuspendEx(true));
            UnSuspendCommand = new RelayCommand(async () => await SuspendEx(false));
        }

        public RelayCommand SuspendCommand { get; set; }
        public RelayCommand UnSuspendCommand { get; set; }
        public ObservableCollection<string> Grades { get; set; }

        private async Task SuspendEx(bool value)
        {
            foreach (var assure in ItemsList.AsEnumerable().Where(x => x.IsCheck))
            {
                assure.Suspende = value;
                Context.Assures.AddOrUpdate(assure);
            }
            await Context.SaveChangesAsync();
        }

        public override async Task AsyncIntialiser()
        {
            try
            {
                await NavigateTo(typeof(ShowClientsView));
                Grades =
                    new ObservableCollection<string>(
                        await Context.Assures.GroupBy(c => c.Grade).Select(x => x.Key).ToListAsync());
                await base.AsyncIntialiser();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
                //
            }
        }

        public override async Task SearchEx()
        {
            try
            {
                if (SearchPattern.IsNullOrEmpty())
                    SearchExpression = assure => true;
                else if (SearchPattern.IsNumeric())
                    SearchExpression = client => client.Matricule.Contains(SearchPattern);
                else
                    SearchExpression =
                        client =>
                            (client.Nom + " " + client.Prenom).Contains(SearchPattern) ||
                            (client.Prenom + " " + client.Nom).Contains(SearchPattern);
                ItemsList = Context.Assures.Where(SearchExpression)
                    .OrderBy(assure => assure.Nom)
                    .Select(x => new Dto.Assure
                    {
                        Matricule = x.Matricule,
                        Nom = x.Nom,
                        Prenom = x.Prenom,
                        DateNaissance = x.DateNaissance,
                        Id = x.Id,
                        Suspende = x.Suspende,
                        Patients = x.Patients,
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
                if (SelectedItem == null) return;
                if (SelectedItem?.Matricule?.Length < 1)
                {
                    await ShowErrorMessage("saisi le Matricule ");
                    return;
                }
                if (SelectedItem?.Id == 0 && await Context.Assures.AnyAsync(a => a.Matricule == SelectedItem.Matricule))
                {
                    await ShowMessage("Avertissement", "MatriculeExist");
                    return;
                }
                await base.SaveEx();
                AddAssureToPatients(SelectedItem.Matricule);
                if (Locator.VentViewModel.Matricule == SelectedItem.Matricule)
                    await Locator.VentViewModel?.SearchClientEx(SelectedItem.Matricule);
                await SearchEx();
                Messenger.Default.Send(new ObservableCollection<Assure>());
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
                await ShowErrorMessage(ex.ToString());
            }
        }

        private async void AddAssureToPatients(string matricule)
        {
            try
            {
                if (Context.Patients.Any(x => x.Alliance == 0 && x.Assure.Matricule == matricule)) return;
                var assure = Context?.Assures?.Include(x => x.Patients).FirstOrDefault(x => x.Matricule == matricule);
                if (assure == null) return;
                var p = new Patient
                {
                    AssureId = assure.Id,
                    Alliance = 0,
                    Nom = assure?.Nom,
                    Prenom = assure?.Prenom,
                    DateNaissance = assure?.DateNaissance,
                    Au = assure.Jusquea,
                    Du = DateTime.Now,
                    Suspende = assure.Suspende
                };
                Context?.Patients?.Add(p);
                await Context?.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }

        }

        public override async Task AddEx()
        {
            await base.AddEx();
            await NavigateTo(typeof(ClientDetailsView));
        }

        public override async Task UpdateEx(int id = 0)
        {
            await base.UpdateEx(id);
            await NavigateTo(typeof(ClientDetailsView));
        }

        public async Task AddEx(string matricule)
        {
            await base.AddEx();
            SelectedItem.Matricule = matricule;
        }
    }
}