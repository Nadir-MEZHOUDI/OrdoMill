using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Npgsql;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.DbConnector
{
    [AddINotifyPropertyChangedInterface]
    public class DbConnectorVm : ViewModelWithDialogs
    {
        private static readonly List<string> InsatncesList = new List<string>
        {
            "localhost",
            "127.0.0.1",
            LocalMachin
        };

        private readonly IDatabaseService databaseService;

        public DbConnectorVm(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            BrowsDbCommand = new RelayCommand(BrowseDb_Ex);
            CheckConCommand = new RelayCommand(async () => await CheckCon_Ex(), CheckCon_CanEx);
            GetNetworkPCs = new RelayCommand(async () => await GetNetworkPcs());
            GetServerDBs = new RelayCommand(async () => await GetServerDbs());
            ResetConfigCommand = new RelayCommand(async () => await RestCon());
            SaveConfigCommand = new RelayCommand(async () => await SaveConfig_Ex());
            Initial = Initializer();
        }



        public bool IsInNetwork { get; set; }

        public RelayCommand GetServerDBs { get; set; }

        public Task Initial { get; set; }

        public RelayCommand BrowsDbCommand { get; set; }

        public RelayCommand CheckConCommand { get; set; }

        public bool Create { get; set; }

        public ObservableCollection<string> Databases { get; set; }

        public string DbName { get; set; }

        public string DbPath { get; set; }

        public bool Delete { get; set; }

        public RelayCommand GetNetworkPCs { get; set; }

        public bool IntegratedSecurity { get; set; }

        public bool IsAttached { get; set; }

        public bool IsNotAttached => !IsAttached;

        public static string LocalMachin => Environment.MachineName;

        public ObservableCollection<string> NetworkPCs { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public RelayCommand ResetConfigCommand { get; set; }

        public RelayCommand SaveConfigCommand { get; set; }

        public string ServerName { get; set; }

        public bool SqlSecurity => !IntegratedSecurity;

        public ObservableCollection<string> SuggestedList { get; set; }

        public string UserName { get; set; }

        public bool IsSearchingPCs { get; set; }

        public bool IsSearchingDbs { get; set; }

        public DbContext Context { get; set; }

        private async Task Initializer()
        {
            SuggestedList = new ObservableCollection<string>(InsatncesList);
            await GetConnectionFields();
            await GetNetworkPcs();
        }

        private async Task SaveConfig_Ex()
        {
            try
            {
                Settings.Default.ConnectionString = await BuildConnectionStr();
                Settings.Default.Save();
                await ShowMessage("تهانينا", "تم حفظ الإعدادات بنجاح");
            }
            catch (Exception)
            {
                // ex
            }
        }

        private async Task<string> BuildConnectionStr()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = string.IsNullOrWhiteSpace(ServerName) ? "localhost" : ServerName.Trim(),
                Port = Port > 0 ? Port : 5432,
                Database = string.IsNullOrWhiteSpace(DbName) ? "ordomill" : DbName.Trim()
            };

            if (!IntegratedSecurity)
            {
                builder.Username = UserName?.Trim();
                builder.Password = Password ?? string.Empty;
            }

            return await Task.FromResult(builder.ConnectionString);
        }

        private void BrowseDb_Ex()
        {
            var op = new OpenFileDialog { Filter = @"All files|*.*" };
            if (op.ShowDialog() == true)
            {
                DbPath = op.FileName;
                var a = new FileInfo(DbPath);
                DbName = Path.GetFileNameWithoutExtension(a.Name);
            }
        }

        private bool CheckCon_CanEx()
        {
            return (IntegratedSecurity || (UserName.IsNotNullOrEmpty() && Password.IsNotNullOrEmpty())) &&
                   ((!IsAttached && DbName.IsNotNullOrEmpty()) || (IsAttached && DbPath.IsNotNullOrEmpty()));
        }

        private async Task CheckCon_Ex()
        {
            var controller =
                await ShowProgressMessage("اتصال", "فضلا انتظر يتم الآن ربط الاتصال بقاعدة البيانات", false);
            controller.SetIndeterminate();
            try
            {
                var strCon = await BuildConnectionStr();
                Context = new DbCon(strCon);
                // Context.Database.Connection.ConnectionString = strCon;

                if (Delete)
                    await Context.Database.EnsureDeletedAsync();
                if (Create)
                {
                    await Context.Database.EnsureCreatedAsync();
                }
                var isDbExist = await Context.Database.CanConnectAsync();
                if (isDbExist)
                    await ShowMessage("تهانينا", "تم الإتصال بقاعدة البيانات بنجاح");
                else
                    await ShowFinishMessage("خطأ", "لا يمكن الاتصال بقاعدة البيانات تأكد من البيانات ثم أعد المحاولة");
            }
            catch (InvalidOperationException)
            {
                await ShowFinishMessage("عذرا", @" قاعدة البيانات موجودة مسبقا ");
            }
            catch (Exception ex)
            {
                await ShowFinishMessage("عذرا", $@"لا يمكن اجراء الاتصال {Environment.NewLine}{ex}");
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        private async Task GetConnectionFields() => await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.ConnectionString))
            {
                ServerName = "localhost";
                Port = 5432;
                DbName = "ordomill";
                IntegratedSecurity = false;
                IsAttached = false;
                return;
            }

            var builder = new NpgsqlConnectionStringBuilder(Settings.Default.ConnectionString);

            ServerName = string.IsNullOrWhiteSpace(builder.Host) ? "localhost" : builder.Host;
            IsInNetwork = ServerName != "localhost" && ServerName != "127.0.0.1" && ServerName != LocalMachin;
            Port = builder.Port > 0 ? builder.Port : 5432;
            IntegratedSecurity = string.IsNullOrWhiteSpace(builder.Username) && string.IsNullOrWhiteSpace(builder.Password);

            if (!IntegratedSecurity)
            {
                UserName = builder.Username;
                Password = builder.Password;
            }

            IsAttached = false;
            DbPath = string.Empty;
            DbName = string.IsNullOrWhiteSpace(builder.Database) ? "ordomill" : builder.Database;
        });

        private async Task GetNetworkPcs()
        {
            IsSearchingPCs = true;
            NetworkPCs = new ObservableCollection<string>();
            var list = await databaseService.GetAllPCsOnLocalNetworkAsync();
            foreach (var pc in list)
                NetworkPCs.Add(pc);
            IsSearchingPCs = false;
        }

        private async Task GetServerDbs()
        {
            IsSearchingDbs = true;
            Databases = new ObservableCollection<string>();
            var list = await databaseService.GetDatabasesFromServerAsync(ServerName);
            foreach (var db in list)
                Databases.Add(db);
            IsSearchingDbs = false;
        }

        private async Task RestCon()
        {
            DbName = "OrdoMill_Db";
            IntegratedSecurity = false;
            IsAttached = false;
            Port = 5432;
            ServerName = "localhost";
            await ShowMessage("تم", "تم وضع الإعدادات الافتراضية");

        }
    }

    //public class DbConnectorVm : NavigableViewModel
    //{
    //    //public string DatabaseFolder { get; set; }
    //    public static string ConnectionString => $@"Server={Settings.Default.ServerName};AttachDbFilename={Settings.Default.dbPath};Database={Settings.Default.dbName};Trusted_Connection=Yes;";
    //    public bool AttachedDb { get; set; } = true;
    //    public string DbPath { get; set; }
    //    public RelayCommand TestCommand { get; set; }
    //    public RelayCommand<Window> ValiderCommand { get; set; }
    //    public RelayCommand SelectFileCommand { get; set; }
    //    public bool SaveConfig { get; set; }

    //    public string ServerName { get; set; }

    //    public DbConnectorVm()
    //    {
    //        TestCommand = new RelayCommand(Test_Execute);
    //        ValiderCommand = new RelayCommand<Window>(Valider_Ex);
    //        SelectFileCommand = new RelayCommand(SelectFile_Ex);
    //        DbPath = Settings.Default.dbPath;
    //    }

    //    private void SelectFile_Ex()
    //    {
    //        if (AttachedDb)
    //        {
    //            var op = new OpenFileDialog();
    //            if (op.ShowDialog() == true)
    //                DbPath = op.FileName;
    //        }
    //        else
    //        {
    //            var op = new FolderBrowserDialog();
    //            if (op.ShowDialog() == DialogResult.OK)
    //                DbPath = op.SelectedPath + "\\OrdoMill.mdf";
    //        }
    //    }
    //    private void Valider_Ex(Window window)
    //    {
    //        if (SimpleIoc.Default.IsRegistered<DbCon>())
    //            SimpleIoc.Default.Unregister<DbCon>();

    //        SimpleIoc.Default.Register(() => new DbCon(Settings.Default.dbPath));

    //        if (SaveConfig)
    //        {
    //            Settings.Default.dbPath = DbPath;
    //            Settings.Default.Save();
    //        }
    //        window.Close();
    //    }


    //    private async void Test_Execute()
    //    {
    //        try
    //        {
    //            var db = new DbCon(DbPath);
    //            if (!db.Database.Exists())
    //            {
    //                Database.SetInitializer(new DbInitializer());
    //                db.Database.Initialize(true);
    //            }
    //            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbCon, Configuration>(true));
    //            db.Database.Initialize(true);

    //            bool result = db.Database.Exists();
    //            await ShowMessage(this, result ? "Succès" : "Erreur", result ? @"La base de données connecté" : @"Erreur de connexion", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = result ? MetroDialogColorScheme.Theme : MetroDialogColorScheme.Accented });
    //        }
    //        catch (Exception ex)
    //        {
    //            await ShowMessage(this, "Error", @"Exception Test InValid" + Environment.NewLine + ex);
    //        }
    //    }
    //}
}
