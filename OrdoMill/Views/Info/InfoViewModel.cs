using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using OrdoMill.Services;
using PropertyChanged;

namespace OrdoMill.Views.Info
{
    [AddINotifyPropertyChangedInterface]
    public sealed class InfoViewModel : Repository<Data.Model.Info, InfoView, InfoView>
    {
        public InfoViewModel()
        {
            IsEditable = true;
        }
 

        public override async Task SearchEx()
        {
            try
            {
                SelectedItem = await Context.Infos.FirstOrDefaultAsync() ?? new Data.Model.Info { Id = 1 };
                IsEditable = true;
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
                if (!await Context.Infos.AnyAsync())
                {
                    Context.Infos.Add(SelectedItem);
                    await Context.SaveChangesAsync();
                }
                else
                {
                    SelectedItem.Id = 1;
                    await base.SaveEx();
                    WeakReferenceMessenger.Default.Send(SelectedItem);
                }
                Locator.PharmacieInfo = SelectedItem;
                Locator.Main.IsInfoFlyoutOpen = false;
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }

        }
    }


}