using System.Windows;
using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using OrdoMill.Data.Model;
using OrdoMill.Interfaces;
using OrdoMill.Properties;
using OrdoMill.Services;
using OrdoMill.Views.About;
using OrdoMill.Views.AllSettings;
using OrdoMill.Views.Bordereau;
using OrdoMill.Views.Clients;
using OrdoMill.Views.DbConnector;
using OrdoMill.Views.Home;
using OrdoMill.Views.Info;
using OrdoMill.Views.Medecins;
using OrdoMill.Views.Medicaments;
using OrdoMill.Views.Ordonnance;
using OrdoMill.Views.Patients;
using OrdoMill.Views.Specialities;
using OrdoMill.Views.Statistics;
using OrdoMill.Views.ThemeChanger;
using OrdoMill.Views.Users;
using OrdoMill.Views.Vente;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.ViewModel
{
    [ImplementPropertyChanged]
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            //if (!SimpleIoc.Default.IsRegistered<EmailSender>())
            //    SimpleIoc.Default.Register(() =>
            //        new EmailSender("nadirelghazali@gmail.com", "pass",
            //            Settings.Default.ClientEMail ?? "nadirelghazali@gmail.com"));
            if (!SimpleIoc.Default.IsRegistered<IDialogCoordinator>())

                SimpleIoc.Default.Register(() => MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance);

            if (!SimpleIoc.Default.IsRegistered<DbCon>())
                SimpleIoc.Default.Register(() => new DbCon(Settings.Default.ConnectionString));

            if (!SimpleIoc.Default.IsRegistered<IMapper>())
                SimpleIoc.Default.Register(CreatMap);

            if (!SimpleIoc.Default.IsRegistered<IUnitOfWork>())
                SimpleIoc.Default.Register<IUnitOfWork, UnitOfWork>();

            SimpleIoc.Default.Register<MainPage>();
            SimpleIoc.Default.Register<StaticServices>();
            SimpleIoc.Default.Register<VentViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ClientsViewModel>();
            SimpleIoc.Default.Register<MedecinsViewModel>();
            SimpleIoc.Default.Register<PathologiesViewModel>();
            SimpleIoc.Default.Register<MedicamentsViewModel>();
            SimpleIoc.Default.Register<OrdonnanceViewModel>();
            SimpleIoc.Default.Register<InfoViewModel>();
            SimpleIoc.Default.Register<PatientsViewModel>();
            SimpleIoc.Default.Register<OrdoViewModel>();
            SimpleIoc.Default.Register<PrintViewModel>();
            SimpleIoc.Default.Register<Ordonnance>();
            SimpleIoc.Default.Register<ChangeThemeVm>();
            SimpleIoc.Default.Register<UsersViewModel>();
            SimpleIoc.Default.Register<FacturesViewModel>();
            SimpleIoc.Default.Register<BordereauxViewModel>();
            SimpleIoc.Default.Register<ContactUsViewModel>();
            SimpleIoc.Default.Register<LicenseViewModel>();
            SimpleIoc.Default.Register<IDatabaseService, DatabaseService>();
            SimpleIoc.Default.Register<DbConnectorVm>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<StatisticsViewModel>();
        }

        public ViewModelLocator()
        {
            Messenger.Default?.Register<Ordonnance>(this, msg => SelectedOrdonnance = msg);
        }

        public IMapper Mapper => ServiceLocator.Current.GetInstance<IMapper>();


        //public EmailSender EmailSender => ServiceLocator.Current.GetInstance<EmailSender>();

        public static ViewModelLocator Instance => Application.Current.Resources["Locator"] as ViewModelLocator;

        public StaticServices StaticServices => ServiceLocator.Current.GetInstance<StaticServices>();

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public ClientsViewModel ClientViewModel => ServiceLocator.Current.GetInstance<ClientsViewModel>();

        public MedecinsViewModel MedecinsViewModel => ServiceLocator.Current.GetInstance<MedecinsViewModel>();

        public PathologiesViewModel PathologiesViewModel => ServiceLocator.Current.GetInstance<PathologiesViewModel>();

        public MedicamentsViewModel MedicamentsViewModel => ServiceLocator.Current.GetInstance<MedicamentsViewModel>();

        public VentViewModel VentViewModel => ServiceLocator.Current.GetInstance<VentViewModel>();

        public OrdonnanceViewModel OrdonancesViewModel => ServiceLocator.Current.GetInstance<OrdonnanceViewModel>();

        public InfoViewModel InfoViewModel => ServiceLocator.Current.GetInstance<InfoViewModel>();

        public PatientsViewModel PatientsViewModel => ServiceLocator.Current.GetInstance<PatientsViewModel>();
     
        public IDialogCoordinator DialogCoordinator => ServiceLocator.Current.GetInstance<IDialogCoordinator>();

        public UsersViewModel UsersViewModel => ServiceLocator.Current.GetInstance<UsersViewModel>();

        public ContactUsViewModel ContactUsViewModel => ServiceLocator.Current.GetInstance<ContactUsViewModel>();

        public Info PharmacieInfo { get; set; }

        public Ordonnance SelectedOrdonnance { get; set; }

        public PatientsViewModel VentPatientsViewModel => ServiceLocator.Current.GetInstance<PatientsViewModel>("Vent");

        public ClientsViewModel VentClientsViewModel => ServiceLocator.Current.GetInstance<ClientsViewModel>("Vent");

        public MedicamentsViewModel VentMedicamentViewModel
            => ServiceLocator.Current.GetInstance<MedicamentsViewModel>("Vent");

        public DbConnectorVm DbConnectorVm => ServiceLocator.Current.GetInstance<DbConnectorVm>();
        public MainPage MainView => ServiceLocator.Current.GetInstance<MainPage>();
        public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();
        public StatisticsViewModel StatisticsViewModel => ServiceLocator.Current.GetInstance<StatisticsViewModel>();
        public BordereauxViewModel BordereauxViewModel => ServiceLocator.Current.GetInstance<BordereauxViewModel>();
        public FacturesViewModel FacturesViewModel => ServiceLocator.Current.GetInstance<FacturesViewModel>();

        private static IMapper CreatMap()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Assure, Assure>();
                cfg.CreateMap<Bordereau, Bordereau>();
                cfg.CreateMap<Info, Info>();
                cfg.CreateMap<Facture, Facture>();
                cfg.CreateMap<Forme, Forme>();
                cfg.CreateMap<Historique, Historique>();
                cfg.CreateMap<Medecin, Medecin>();
                cfg.CreateMap<Medicament, Medicament>();
                cfg.CreateMap<Pathologie, Pathologie>();
                cfg.CreateMap<MedOrd, MedOrd>();
                cfg.CreateMap<Ordonnance, Ordonnance>();
                cfg.CreateMap<Patient, Patient>();
                cfg.CreateMap<User, User>();
                cfg.CreateMap<Operation, Operation>();

                cfg.CreateMap<Assure, Dto.Assure>();
                cfg.CreateMap<Dto.Assure, Assure>();
                cfg.CreateMap<Patient, Dto.Patient>();
                cfg.CreateMap<Dto.Patient, Patient>();
                cfg.CreateMap<Ordonnance, Dto.Ordonnance>();
                cfg.CreateMap<Dto.Ordonnance, Ordonnance>();
                cfg.CreateMap<Facture, Dto.Facture>();
                cfg.CreateMap<Dto.Facture, Facture>();
                cfg.CreateMap<Bordereau, Dto.Bordereau>();
                cfg.CreateMap<Dto.Bordereau, Bordereau>();
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }
    }
}