using System;
using System.Windows;
using AutoMapper;
using CommunityToolkit.Mvvm.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
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
using OrdoMill.Helpers;

namespace OrdoMill.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ViewModelLocator
    {
        private static IServiceProvider _serviceProvider;
        private static readonly object _lock = new object();

        static ViewModelLocator()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDialogCoordinator>(sp => MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance);
            services.AddTransient<DbCon>(sp => new DbCon(Settings.Default.ConnectionString));
            services.AddSingleton<IMapper>(sp => CreateMap());
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<MainPage>();
            services.AddTransient<StaticServices>();
            services.AddTransient<VentViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ClientsViewModel>();
            services.AddTransient<MedecinsViewModel>();
            services.AddTransient<PathologiesViewModel>();
            services.AddTransient<MedicamentsViewModel>();
            services.AddTransient<OrdonnanceViewModel>();
            services.AddTransient<InfoViewModel>();
            services.AddTransient<PatientsViewModel>();
            services.AddTransient<OrdoViewModel>();
            services.AddTransient<PrintViewModel>();
            services.AddTransient<Ordonnance>();
            services.AddTransient<ChangeThemeVm>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<FacturesViewModel>();
            services.AddTransient<BordereauxViewModel>();
            services.AddTransient<ContactUsViewModel>();
            services.AddTransient<LicenseViewModel>();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<DbConnectorVm>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<StatisticsViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        public ViewModelLocator()
        {
            WeakReferenceMessenger.Default?.Register<Data.Model.Ordonnance>(this, (r, msg) => SelectedOrdonnance = msg);
        }

        public IMapper Mapper => _serviceProvider.GetRequiredService<IMapper>();

        public static ViewModelLocator Instance => Application.Current.Resources["Locator"] as ViewModelLocator;

        public StaticServices StaticServices => _serviceProvider.GetRequiredService<StaticServices>();

        public MainViewModel Main => _serviceProvider.GetRequiredService<MainViewModel>();

        public ClientsViewModel ClientViewModel => _serviceProvider.GetRequiredService<ClientsViewModel>();

        public MedecinsViewModel MedecinsViewModel => _serviceProvider.GetRequiredService<MedecinsViewModel>();

        public PathologiesViewModel PathologiesViewModel => _serviceProvider.GetRequiredService<PathologiesViewModel>();

        public MedicamentsViewModel MedicamentsViewModel => _serviceProvider.GetRequiredService<MedicamentsViewModel>();

        public VentViewModel VentViewModel => _serviceProvider.GetRequiredService<VentViewModel>();

        public OrdonnanceViewModel OrdonancesViewModel => _serviceProvider.GetRequiredService<OrdonnanceViewModel>();

        public InfoViewModel InfoViewModel => _serviceProvider.GetRequiredService<InfoViewModel>();

        public PatientsViewModel PatientsViewModel => _serviceProvider.GetRequiredService<PatientsViewModel>();
      
        public IDialogCoordinator DialogCoordinator => _serviceProvider.GetRequiredService<IDialogCoordinator>();

        public UsersViewModel UsersViewModel => _serviceProvider.GetRequiredService<UsersViewModel>();

        public ContactUsViewModel ContactUsViewModel => _serviceProvider.GetRequiredService<ContactUsViewModel>();

        public Data.Model.Info PharmacieInfo { get; set; }

        public Data.Model.Ordonnance SelectedOrdonnance { get; set; }

        public PatientsViewModel VentPatientsViewModel => _serviceProvider.GetRequiredService<PatientsViewModel>();

        public ClientsViewModel VentClientsViewModel => _serviceProvider.GetRequiredService<ClientsViewModel>();

        public MedicamentsViewModel VentMedicamentViewModel => _serviceProvider.GetRequiredService<MedicamentsViewModel>();

        public DbConnectorVm DbConnectorVm => _serviceProvider.GetRequiredService<DbConnectorVm>();
        public MainPage MainView => _serviceProvider.GetRequiredService<MainPage>();
        public SettingsViewModel SettingsViewModel => _serviceProvider.GetRequiredService<SettingsViewModel>();
        public StatisticsViewModel StatisticsViewModel => _serviceProvider.GetRequiredService<StatisticsViewModel>();
        public BordereauxViewModel BordereauxViewModel => _serviceProvider.GetRequiredService<BordereauxViewModel>();
        public FacturesViewModel FacturesViewModel => _serviceProvider.GetRequiredService<FacturesViewModel>();

        private static IMapper CreateMap()
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
