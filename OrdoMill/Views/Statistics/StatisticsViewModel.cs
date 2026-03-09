using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Services;

namespace OrdoMill.Views.Statistics
{
    public class StatisticsViewModel : NavigableViewModel
    {
        public StatisticsViewModel()
        {
            Context = new DbCon(Settings.Default.ConnectionString);            
            var task = Intilizer();
        }

        public ObservableCollection<Chart> Charts { get; set; }
        public DbCon Context { get; }

        public async Task Intilizer()
        {
            Charts = new ObservableCollection<Chart>(new[]
            {
                new Chart
                {
                    Title = "Top 50 Medicaments rotation / an",
                    SubTitle = "Année 2016",
                    Series = new ObservableCollection<Serie>(
                        await Context.MedOrds.GroupBy(m => m.Medicament.Dci)
                            .Select(g => new Serie {Name = g.Key, Count = g.Count()})
                            .OrderByDescending(serie => serie.Count)
                            .Take(20).ToListAsync()
                    )
                },
                new Chart
                {
                    Title = "Top 50 Medicaments rotation / an",
                    SubTitle = "Année 2016",
                    Series =new ObservableCollection<Serie>(await Context.MedOrds
                        .GroupBy(m => m.Medicament)
                        .Select(g => new Serie {Name = g.Key.Nom, Count = g.Count()})
                        .OrderByDescending(serie => serie.Count)
                        .Take(20).ToListAsync())


                },
                new Chart
                {
                    Title = "Top 50 Medicaments rotation / an",
                    SubTitle = "Année 2016",
                    Series = new ObservableCollection<Serie>(await Context.MedOrds
                        .GroupBy(m => m.Ordonnance.Patient.Assure)
                        .Select(g => new Serie {Name = g.Key.Nom + " " + g.Key.Prenom, Count = g.Count()})
                        .OrderByDescending(serie => serie.Count)
                        .Take(20).ToListAsync())
                }
            });
        }
    }

    public class Serie
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class Chart
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public ObservableCollection<Serie> Series { get; set; }
    }
}