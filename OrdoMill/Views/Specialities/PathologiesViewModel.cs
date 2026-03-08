using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Specialities
{
    public class PathologiesViewModel : Repository<Pathologie, SpecialitesView, SpecialitesView>
    {

        public override async Task SearchEx()
        {
            SearchExpression = SearchPattern.IsNullOrEmpty()
                ? (Expression<Func<Pathologie, bool>>)(medecin => true)
                : (specialite => specialite.Nom.Contains(SearchPattern) || specialite.Code.Contains(SearchPattern));
            ItemsList = Context.Pathologies.Where(SearchExpression).OrderBy(x => x.Code)
            //.Select(x => new Dto.Pathologie
            //{
            //    Nom = x.Nom,
            //    Id = x.Id,
            //    Code = x.Code
            //})
            ;
            await BaseSearchEx();

            SelectedItem = null;
            await Task.Delay(10);
        }

        public override async Task SaveEx()
        {
            await base.SaveEx();
            WeakReferenceMessenger.Default.Send(new ObservableCollection<Pathologie>());
        }
    }
}
