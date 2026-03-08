using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Interfaces;
using OrdoMill.Properties;
using OrdoMill.ViewModel;
using PropertyChanged;
using SmartApp.Helpers.Bases;
using SmartApp.Helpers.Helpers;
using static MahApps.Metro.Controls.Dialogs.MessageDialogStyle;

namespace OrdoMill.Services
{
    [AddINotifyPropertyChangedInterface]
    public abstract class Repository<TEntity, THomeView, TDetailsView> : NavigableViewModel, IRepository<TEntity> where TEntity : class, IEntity, new() where THomeView : class where TDetailsView : class
    {
        private CancellationTokenSource searchCts;

        //TODO Change Row Color To Red If Error I ORdo and Suspended Clients
        protected Repository()
        {
            Context = new DbCon(Settings.Default.ConnectionString);
            Context.Configuration.AutoDetectChangesEnabled = false;
            SearchExpression = entity => true;
            ListToDisplay = new ObservableCollection<TEntity>();
            AddCommand = new RelayCommand(async () => await AddEx(), AddCanEx);
            UpdateCommand = new RelayCommand<int>(async e => await UpdateEx(e), UpdateCanEx);
            DeleteCommand = new RelayCommand<int>(async e => await DeleteEx(e), DeleteCanEx);
            SaveCommand = new RelayCommand(async () => await SaveEx(), SaveCanEx);
            CancelCommand = new RelayCommand(async () => await CancelEx(), CancelCanEx);
            SearchCommand = new RelayCommand(async () => await SearchEx(), SearchCanEx);
            ExportCommand = new RelayCommand(async () => await ExportEx());
            LostFocusCommand = new RelayCommand<bool>(b => IsFocused = false);
            FrstPageCommand = new RelayCommand(async () => await GoFirstPage_Excute(), () => CurrentPageNumber > 2);
            LastPageCommand = new RelayCommand(async () => await GoLastPage_Excute(),
                () => CurrentPageNumber < PagesCount - 1);
            NextPageCommand = new RelayCommand(async () => await GoNextPage_Excute(),
                () => CurrentPageNumber < PagesCount);
            PrevPageCommand = new RelayCommand(async () => await GoPrevPage_Excute(), () => CurrentPageNumber > 1);
            GoToHomePage = new RelayCommand(async () => await NavigateTo(typeof(THomeView)), () => !IsEditable);
            GoToDetailsPage = new RelayCommand<int>(async x => await GoToDetailsPageEx(x), UpdateCanEx);
            SearchPattern = "";
            WeakReferenceMessenger.Default.Register<Tuple<string, int>>(this, (r, m) =>
            {
                if (m.Item1 == "PageSize") PageSize = m.Item2;
            });

            // ReSharper disable once VirtualMemberCallInConstructor
            var initializer = AsyncIntialiser();
        }

        protected DbCon Context { get; private set; }
        public RelayCommand GoToHomePage { get; set; }
        public RelayCommand<int> GoToDetailsPage { get; set; }
        public ObservableCollection<TEntity> ListToDisplay { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public RelayCommand FrstPageCommand { get; set; }
        public RelayCommand LastPageCommand { get; set; }
        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }
        public int PageSize { get; set; } = Settings.Default.PageSize;
        protected static ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModelLocator;
        public int ResultCount { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand AddCommand { get; set; }//= new RelayCommand(async () => await AddEx(), AddCanEx);
        public RelayCommand<int> UpdateCommand { get; set; }
        public RelayCommand<int> DeleteCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<bool> LostFocusCommand { get; set; }
        public virtual async Task ExportEx() => await Task.Delay(1);

        public virtual async Task AsyncIntialiser()
        {
            if (typeof(THomeView) != typeof(TDetailsView))
            {
                await NavigateTo(typeof(THomeView));
            }
            await SearchEx();
        }

        private async Task GoToDetailsPageEx(int id = 0)
        {
            try
            {
                if ((SelectedItem == null) && (id == 0)) return;
                var entity = await Context.Set<TEntity>().FindAsync(id > 0 ? id : SelectedItem?.Id);
                SelectedItem = entity;
                await NavigateTo(typeof(TDetailsView));
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public async Task GoPrevPage_Excute()
        {
            CurrentPageNumber--;
            await GetPageFromServerByNumber();
        }

        public async Task GoNextPage_Excute()
        {
            CurrentPageNumber++;
            await GetPageFromServerByNumber();
        }

        [DebuggerHidden]
        private async Task GetPageFromServerByNumber(int count = 0)
        {
            try
            {
                searchCts?.Cancel();
                searchCts = new CancellationTokenSource();
                var token = searchCts.Token;
                await Task.Delay(TimeSpan.FromMilliseconds(100), token);
                if (count == 0)
                    count = ResultCount;
                if (token.IsCancellationRequested) return;
                ListToDisplay = new ObservableCollection<TEntity>(await
                    ItemsList.PagedResult(count, CurrentPageNumber, PageSize)
                        .ToListAsync(token));
            }
            catch (OperationCanceledException)
            {
                // Expected when pagination is cancelled
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        private async Task GoLastPage_Excute()
        {
            CurrentPageNumber = PagesCount;
            await GetPageFromServerByNumber();
        }

        private async Task GoFirstPage_Excute(int count = 0)
        {
            CurrentPageNumber = 1;
            await GetPageFromServerByNumber(count);
        }

        [DebuggerHidden]
        private Task<Tuple<int, int>> CalculatPagesCount() => Task.Run(async () =>
        {
            searchCts?.Cancel();
            searchCts = new CancellationTokenSource();
            var token = searchCts.Token;
            var result = new Tuple<int, int>(0, 0);
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), token);
                if (token.IsCancellationRequested) return result;
                if (ItemsList == null) return result;
                var resultCount = await ItemsList?.CountAsync(token);
                var pagesCount = resultCount / PageSize;
                if (resultCount % PageSize != 0)
                    pagesCount++;
                result = new Tuple<int, int>(resultCount, pagesCount);
                return result;
            }
            catch (OperationCanceledException)
            {
                // Expected when count calculation is cancelled
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
            return result;
        });

        #region Entity Implementation

        public Expression<Func<TEntity, bool>> SearchExpression { get; set; }
        public bool IsEditable { get; set; }
        public bool IsFocused { get; set; } = true;
        public IQueryable<TEntity> ItemsList { get; set; }
        public string SearchPattern { get; set; }
        public TEntity SelectedItem { get; set; }
        public virtual bool AddCanEx() => !IsEditable;

        public virtual async Task AddEx()
        {
            SelectedItem = new TEntity();
            IsEditable = true;
            IsFocused = true;
            await Task.Delay(1);
        }

        public virtual bool CancelCanEx() => IsEditable;

        public virtual async Task CancelEx()
        {
            try
            {
                IsEditable = false;
                if (SelectedItem?.Id > 0)
                    Context?.RefreshEntry();
                await SearchEx();
            }
            catch (Exception ex)
            {
                SelectedItem = null;
                await ex.AppLoggingAsync();
            }
        }

        public virtual bool DeleteCanEx(int id) => (id > 0) || (!IsEditable && (SelectedItem?.Id > 0));

        public virtual async Task DeleteEx(int id = 0)
        {
            try
            {
                if ((SelectedItem?.Id == 0) && (id == 0)) return;
                {
                    var entity = await Context.Set<TEntity>().FindAsync(id > 0 ? id : SelectedItem?.Id);
                    if (entity == null) return;
                    if (await ShowMessage("Confirmation", "SupprimeMsg", AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    {
                        Context.Set<TEntity>().Remove(entity);
                        await Context.SaveChangesAsync();
                        await SearchEx();
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage("DeleteErrorMgs");
                await ex.AppLoggingAsync();
            }
        }

        public virtual bool SaveCanEx() => IsEditable && (SelectedItem != null);

        public virtual async Task SaveEx()
        {
            try
            {
                if (SelectedItem.Id == 0)
                    SelectedItem.CreatedAt = DateTime.Now;

                SelectedItem.ModifiedAt = DateTime.Now;

                Context.Set<TEntity>().AddOrUpdate(x => x.Id, SelectedItem);
                await Context.SaveChangesAsync();
                IsFocused = false;
                IsEditable = false;
                var msg = ShowFinishMessage("Termin�", "SavedMsg");
                var srch = SearchEx();
                await Task.WhenAll(msg, srch);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
                await ShowErrorMessage("CancelErrorMgs\n" + ex);
            }
        }

        public bool SearchCanEx() => true;
        public abstract Task SearchEx();

        protected async Task BaseSearchEx()
        {
            try
            {
                var a = await CalculatPagesCount();
                ResultCount = a.Item1;
                PagesCount = a.Item2;
                await GoFirstPage_Excute(a.Item1);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public bool UpdateCanEx(int id) => ((id > 0) || (SelectedItem?.Id > 0)) && !IsEditable;

        public virtual async Task UpdateEx(int id = 0)
        {
            try
            {
                if ((SelectedItem == null) && (id == 0)) return;
                SelectedItem = await Context.Set<TEntity>().FindAsync(id > 0 ? id : SelectedItem?.Id);
                IsEditable = true;
                IsFocused = true;

                if (typeof(THomeView) != typeof(TDetailsView))
                    await NavigateTo(typeof(TDetailsView));
            }
            catch (Exception ex)
            {
                IsEditable = false;
                IsFocused = false;
            }
        }

        #endregion Entity Impementation
    }
}