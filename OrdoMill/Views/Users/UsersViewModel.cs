using OrdoMill.Data.Model;
using OrdoMill.Services;
using PropertyChanged;
using OrdoMill.Helpers;

namespace OrdoMill.Views.Users;

[AddINotifyPropertyChangedInterface]
public class UsersViewModel : Repository<User, UsersView, UsersView>
{
    public override async Task SearchEx()
    {
        try
        {
            if (SearchPattern.IsNullOrEmpty()) SearchExpression = medecin => true;
            else SearchExpression = medecin => medecin.FullName.Contains(SearchPattern);
            ItemsList = Context.Users.Where(SearchExpression).OrderBy(x => x.Id)
                //.Select(x => new Dto.User
                //{
                //    FullName = x.FullName,
                //    AllowAdd = x.AllowAdd,
                //    AllowPrintDocs = x.AllowPrintDocs,
                //    AllowUpdate = x.AllowUpdate,
                //    IsAdmin = x.IsAdmin,
                //    Id = x.Id,
                //    IsWork = x.IsWork,
                //    Password = x.Password,
                //    UserName = x.UserName

                //})
                ;
            SelectedItem = null;
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
            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Password))
            {
                if (SelectedItem.Id == 0 || PasswordHasher.NeedsRehash(SelectedItem.Password))
                {
                    SelectedItem.Password = PasswordHasher.HashPassword(SelectedItem.Password);
                }
            }
            await base.SaveEx();
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }
}