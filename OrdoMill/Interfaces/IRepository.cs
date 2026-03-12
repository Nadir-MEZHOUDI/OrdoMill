using System.Linq.Expressions;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;

namespace OrdoMill.Interfaces;

public interface IRepository<TEntity> where TEntity : class, new()  
{
    RelayCommand AddCommand { get; set; }
    RelayCommand CancelCommand { get; set; }
    UserControl CurrentPage { get; set; }
    RelayCommand<int> DeleteCommand { get; set; }
    RelayCommand ExportCommand { get; set; }
    Expression<Func<TEntity, bool>> SearchExpression { get; set; }
    RelayCommand ImportCommand { get; set; }
    bool IsEditable { get; set; }
    bool IsFocused { get; set; }
    IQueryable<TEntity> ItemsList { get; set; }
    RelayCommand<bool> LostFocusCommand { get; set; }
    RelayCommand<Type> NavigateToCommand { get; set; }
    RelayCommand SaveCommand { get; set; }
    RelayCommand SearchCommand { get; set; }
    Dictionary<string, UserControl> NavigationDictionary { get; set; }
    string SearchPattern { get; set; }
    TEntity SelectedItem { get; set; }
    RelayCommand<int> UpdateCommand { get; set; }

    bool AddCanEx();
    Task AddEx();
    Task AsyncIntialiser();
    bool CancelCanEx();
    Task CancelEx();
    bool DeleteCanEx(int id);
    Task DeleteEx(int id);
    Task ExportEx();
    Task NavigateTo(Type pageType);
    bool SaveCanEx();
    Task SaveEx();
    bool SearchCanEx();
    Task SearchEx();
    bool UpdateCanEx(int id);
    Task UpdateEx(int id);

}