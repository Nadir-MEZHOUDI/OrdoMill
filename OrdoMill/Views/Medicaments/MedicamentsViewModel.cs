using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Services;
using PropertyChanged;
using SmartApp.Helpers.Helpers;
using MessageBox = System.Windows.MessageBox;

namespace OrdoMill.Views.Medicaments
{
    [AddINotifyPropertyChangedInterface]
    public class MedicamentsViewModel : Repository<Medicament, ShowMedicamentsView, MedicamentDetailsView>
    {

        public ObservableCollection<string> Dcis { get; set; }

        public ObservableCollection<Forme> Formes { get; set; }

        public ObservableCollection<string> Doses { get; set; }

        public ObservableCollection<string> Boites { get; set; }

        public bool IncludeNc { get; set; } = true;

        public bool IncludeDci { get; set; }

        public bool IncludeDose { get; set; }

        public bool IncludeForm { get; set; }

        public bool IncludeGen { get; set; }


        public override async Task AsyncIntialiser()
        {
            await NavigateTo(typeof(ShowMedicamentsView));
            //Medicaments = (await Context.Medicaments.Include(x => x.Forme).ToListAsync()).ToObservableCollection();
            Boites = new ObservableCollection<string>(await Context.Medicaments.GroupBy(x => x.Dose).Select(c => c.Key).ToListAsync());
            Formes = new ObservableCollection<Forme>(await Context.Formes.ToListAsync());
            Dcis = new ObservableCollection<string>(await Context.Medicaments.GroupBy(x => x.Dci).Select(c => c.Key).ToListAsync());
            Doses = new ObservableCollection<string>(await Context.Medicaments.GroupBy(x => x.Dose).Select(c => c.Key).ToListAsync());
            await base.AsyncIntialiser();
        }

        public override async Task UpdateEx(int id = 0)
        {
            await base.UpdateEx(id);
            await NavigateTo(typeof(MedicamentDetailsView));
        }
        public override async Task AddEx()
        {
            await base.AddEx();
            await NavigateTo(typeof(MedicamentDetailsView));
        }

        public override async Task ExportEx()
        {
            var save = new FolderBrowserDialog();

            if (save.ShowDialog() == DialogResult.OK)
            {
                var controller = await DialogCoordinator.ShowProgressAsync(this, "تصدير ", @"فضلا انتظر يتم الآن التصدير ", false,
                            Statics.MessageSettings);
                controller.SetProgress(0);
                var exportMedPath = save.SelectedPath + "\\Medicaments.xml";
                var exportFormesPath = save.SelectedPath + "\\Forms.xml";

                try
                {
                    var medArray = await Context.Medicaments.ToListAsync();
                    var frmArray = await Context.Formes.ToListAsync();
                    await Serialize.SerializeDataToXmlFileAsync(exportFormesPath, frmArray);
                    await Serialize.SerializeDataToXmlFileAsync(exportMedPath, medArray);
                    await Statics.CreateHistoAsync(OpNames.ExportPeolpesList);
                    await
                        DialogCoordinator.ShowMessageAsync(this, "تهانينا ", @"تم التصدير بنجاح",
                            MessageDialogStyle.Affirmative,
                            Statics.MessageSettings);
                }
                catch (Exception ex)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "خطأ", "حدث خطأ أثناء التصدير",
                        MessageDialogStyle.Affirmative,
                        new MetroDialogSettings
                        {
                            AffirmativeButtonText = "حسنـا",
                            ColorScheme = MetroDialogColorScheme.Inverted
                        });
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public override async Task SearchEx()
        {
            try
            {

                {

                    var dci = IncludeGen ? (await Context.Medicaments.FirstOrDefaultAsync(x => x.Nom.Contains(SearchPattern)))?.Dci : "";
                    if (SearchPattern.IsNullOrEmpty())
                    {
                        SearchExpression = medicament => true;
                    }
                    else
                    {
                        if (IncludeGen)
                        {
                            SearchExpression = medicament => medicament.Dci.Contains(dci);
                        }
                        else
                        {
                            SearchExpression = med =>
                                  ((IncludeNc ? med.Nom + " " : "") +
                                   (IncludeDci ? med.Dci + " " : "") +
                                   (IncludeDose ? med.Dose + " " : "") +
                                   (IncludeForm ? med.Forme.Name : ""))
                                    .Contains(SearchPattern);
                        }
                    }
                    ItemsList = Context.Medicaments.Where(SearchExpression).OrderBy(x => x.Nom)
                        //.Select(x => new Dto.Medicament
                        //{
                        //    Id = x.Id,
                        //    Nom = x.Nom,
                        //    Dci = x.Dci,
                        //    Forme = x.Forme,
                        //    Dose = x.Dose,
                        //    Boite = x.Boite
                        //})
                    ;
                    await BaseSearchEx();

                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage(ex.Message);
            }
        }
    }
}