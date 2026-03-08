using System.Linq;
using System.Threading.Tasks;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using PropertyChanged;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Views.Medecins
{
    [ImplementPropertyChanged]
    public class MedecinsViewModel : Repository<Medecin, ShowMedecinsView, MedecinsDetailsView>
    {          
        public override async Task SearchEx()
        {
            if (SearchPattern.IsNullOrEmpty())
                SearchExpression = medecin => true;
            else
                SearchExpression = medecin => medecin.Nom.Contains(SearchPattern);

            ItemsList = Context.Medecins.Where(SearchExpression).OrderBy(x => x.Id)
                .Select(x => new Dto.Medecin
                {
                    Id = x.Id,
                    Nom = x.Nom,
                    Tel = x.Tel,
                    Type = x.Type,
                    Adresse = x.Adresse
                });
            await BaseSearchEx();
        }
    }
}