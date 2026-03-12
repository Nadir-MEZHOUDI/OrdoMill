using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.Serializables;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using PropertyChanged;
using OrdoMill.Helpers;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
namespace OrdoMill.Views.ImportData
{

    /// <summary>
    ///     Interaction logic for ImportDataView.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class ImportDataView
    {
        public double ProgressIndicator { get; set; }
        public static ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModelLocator;

        public DbCon Context { get; } = new DbCon(Settings.Default.ConnectionString);

        public IDialogCoordinator Coordinator { get; set; }

        public ImportDataView()
        {
            InitializeComponent();
            Coordinator = Locator.DialogCoordinator;
            DataContext = this;
        }


        private async void ImportFormesBtn_OnClick(object sender, RoutedEventArgs e) => await Task.Run(async () =>
        {
            ProgressIndicator = 0;
            var template = new FileInfo(@"Templates\FORME.txt");
            if (!File.Exists(template.FullName))
            {
                File.WriteAllBytes(template.FullName, GlobalResources.Properties.Resources.FORME);
                Thread.Sleep(1000);
            }
            if (!File.Exists(template.FullName))
            {
                MessageBox.Show("Le Ficher Medicaments n’existe pas");
                return;
            }

            var text = File.ReadAllText(template.FullName).Trim().Split('\n');
            var count = text.Length;
            var oledFormes = await Context.Formes.ToListAsync();
            for (var I = 0; I < count; I++)
            {
                var a = text[I];
                if (a.Length < 50) return;
                var forme = new Forme
                {
                    Id = a.Substring(0, 3).TrimAll().ToInt(),
                    Name = a.Substring(3, 50).TrimAll() ?? "",
                    Abrg = a.Substring(52).TrimAll() ?? ""
                };
                var oledForme = oledFormes.FirstOrDefault(x => x.Name == forme.Name || x.Abrg == forme.Abrg);
                if (oledForme != null)
                    forme.Id = oledForme.Id;

                Context.Formes.AddOrUpdate(forme);
                ProgressIndicator = (double)100 * I / count;
            }
            await Context.SaveChangesAsync();
            MessageBox.Show("Terminé");
        });

        private async void ImportSpecialitesBtn_OnClick(object sender, RoutedEventArgs e) => await Task.Run(async () =>
        {
            ProgressIndicator = 0;
            var template = new FileInfo(@"Templates\PathologiesU.txt");
            if (!File.Exists(template.FullName))
            {
                File.WriteAllBytes(template.FullName, GlobalResources.Properties.Resources.PathologiesU);
                Thread.Sleep(1000);
            }
            if (!File.Exists(template.FullName))
            {
                MessageBox.Show("Le Ficher Medicaments n’existe pas");
                return;
            }

            var text = File.ReadAllText(template.FullName).Trim().Split('\n');
            var count = text.Length;
            // int news = 0, oleds = 0;
            var oledPathologiesList = await Context.Pathologies.ToListAsync();
            for (var I = 0; I < count; I++)
            {
                try
                {
                    if (text[I].Length < 5) return;
                    var a = text[I].Split(',');
                    if (a.Length < 2) return;
                    var pathologie = new Pathologie
                    {
                        Code = a[0]?.TrimAll() ?? "",
                        Nom = a[1]?.TrimAll() ?? ""
                    };
                    var oled = await Context.Pathologies.FirstOrDefaultAsync(x => x.Code == pathologie.Code);
                    if (oled != null)
                        pathologie.Id = oled.Id;
                    Context.Pathologies.AddOrUpdate(pathologie);

                    ProgressIndicator = (double)100 * I / count;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                await Context.SaveChangesAsync();
            }
            MessageBox.Show("Terminé");
        });

        private async void ImportMedicamentsBtn_OnClick(object sender, RoutedEventArgs e) => await Task.Run(async () =>
            {
                ProgressIndicator = 0;

                try
                {
                    var meds = new List<Medicament>();
                    var adjustedFormes = new List<string>();
                    var template = new FileInfo(@"Templates\MEDIC.txt");
                    if (!File.Exists(template.FullName))
                    {
                        File.WriteAllBytes(template.FullName, GlobalResources.Properties.Resources.MEDIC);
                        Thread.Sleep(1000);
                    }
                    if (!File.Exists(template.FullName))
                    {
                        MessageBox.Show("Le Ficher Medicaments n’existe pas");
                        return;
                    }

                    var formeTemplate = new FileInfo(@"Templates\FORME.txt");
                    if (!File.Exists(formeTemplate.FullName))
                    {
                        File.WriteAllBytes(formeTemplate.FullName, GlobalResources.Properties.Resources.FORME);
                        Thread.Sleep(1000);
                    }

                    var existingFormes = await Context.Formes.AsNoTracking().ToListAsync();
                    var formeById = existingFormes.ToDictionary(x => x.Id, x => x.Id);
                    var formeByBusinessKey = existingFormes
                        .Where(x => !string.IsNullOrWhiteSpace(x.Name) || !string.IsNullOrWhiteSpace(x.Abrg))
                        .GroupBy(x => BuildFormeBusinessKey(x.Name, x.Abrg))
                        .ToDictionary(g => g.Key, g => g.First().Id);

                    var sourceToTargetFormeIds = new Dictionary<int, int>();
                    if (File.Exists(formeTemplate.FullName))
                    {
                        var formeLines = File.ReadAllText(formeTemplate.FullName).Trim().Split('\n');
                        foreach (var line in formeLines)
                        {
                            if (line.Length < 53)
                            {
                                continue;
                            }

                            var sourceId = line.Substring(0, 3).TrimAll().ToInt();
                            if (sourceId <= 0)
                            {
                                continue;
                            }

                            if (formeById.ContainsKey(sourceId))
                            {
                                sourceToTargetFormeIds[sourceId] = sourceId;
                                continue;
                            }

                            var formeName = line.Substring(3, 50).TrimAll() ?? "";
                            var formeAbrg = line.Substring(52).TrimAll() ?? "";
                            var businessKey = BuildFormeBusinessKey(formeName, formeAbrg);
                            if (formeByBusinessKey.TryGetValue(businessKey, out var existingId))
                            {
                                sourceToTargetFormeIds[sourceId] = existingId;
                            }
                        }
                    }

                    var text = File.ReadAllText(template.FullName).Trim().Split('\n');
                    var count = text.Length;
                    for (var I = 0; I < count; I++)
                    {
                        try
                        {
                            var a = text[I];
                            if (a.Length < 200) return;
                            var sourceFormeId = a.Substring(212, 6).ToInt();
                            int? resolvedFormeId = null;
                            if (sourceFormeId > 0)
                            {
                                if (formeById.ContainsKey(sourceFormeId))
                                {
                                    resolvedFormeId = sourceFormeId;
                                }
                                else if (sourceToTargetFormeIds.TryGetValue(sourceFormeId, out var mappedFormeId))
                                {
                                    resolvedFormeId = mappedFormeId;
                                }
                            }

                            var med1 = new Medicament
                            {
                                CnasId = a?.Substring(0, 5)?.TrimAll() ?? "",
                                Nom = a.Substring(5, 50).TrimAll() ?? "",
                                Dci = a.Substring(55, 50).TrimAll() ?? "",
                                Dose = a.Substring(105, 30).TrimAll() ?? "",
                                Unite = a.Substring(135, 20).TrimAll() ?? "",
                                Boite = a.Substring(155, 11).TrimAll() ?? "",
                                Tr = a.Substring(193, 19).TrimAll() ?? "",
                                FormeId = resolvedFormeId,
                                Remboursable = true,
                                Controle = false
                            };

                            if (sourceFormeId > 0 && !resolvedFormeId.HasValue)
                            {
                                adjustedFormes.Add($"CNAS={med1.CnasId}, Nom={med1.Nom}, FormeSource={sourceFormeId}");
                            }

                            meds.Add(med1);
                            ProgressIndicator = (double)100 * I / count;
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                    Context.Medicaments.AddRange(meds);
                    await Context.SaveChangesAsync();

                    if (adjustedFormes.Count > 0)
                    {
                        var adjustedPreview = string.Join(Environment.NewLine, adjustedFormes.Take(10));
                        MessageBox.Show(
                            $"{adjustedFormes.Count} medicament(s) importes avec FormeId null (forme introuvable)." +
                            Environment.NewLine +
                            adjustedPreview,
                            "Import Medicaments");
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    await dbEx.AppLoggingAsync();
                    MessageBox.Show("Echec import medicaments: contrainte FK FormeId invalide.");
                }
                catch (Exception ex)
                {
                    await ex.AppLoggingAsync();
                }

                MessageBox.Show("Terminé");
            });

        private static string BuildFormeBusinessKey(string name, string abrg)
        {
            var normalizedName = (name ?? string.Empty).TrimAll().ToUpperInvariant();
            var normalizedAbrg = (abrg ?? string.Empty).TrimAll().ToUpperInvariant();
            return normalizedName + "|" + normalizedAbrg;
        }

        private async void ExportAssuresBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressIndicator = 0;

            var save = new OpenFolderDialog();

            if (save.ShowDialog() == true)
            {
                var exportMedPath = save.FolderName + "\\Assures.xml";
                try
                {
                    var assurSers =
                        (await Context.Assures.Include(x => x.Patients).ToListAsync()).Select(AssureToSerializable);
                    await Serialize.SerializeDataToXmlFileAsync(exportMedPath, assurSers);
                    MessageBox.Show("تهانينا ", @"تم التصدير بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void ImportAssuresBtn_OnClick(object sender, RoutedEventArgs e) => await Task.Run(async () =>
        {
            ProgressIndicator = 0;

            var open = new OpenFileDialog();

            if (open.ShowDialog() == true)
            {
                var exportMedPath = open.FileName;
                try
                {
                    var assurSers = await Serialize.DeSerializeXmlToDataAsync<List<SerializableAssure>>(open.FileName);
                    int count = 0;
                    foreach (var assurSer in assurSers.Where(assurSer => !Context.Assures.Any(x => x.Matricule == assurSer.Matrecul)))
                    {
                        try
                        {
                            Context.Assures.Add(SerializableToAssure(assurSer));
                            count++;
                            await Context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {

                        }
                    }

                    MessageBox.Show("تهانينا ", @"تم الإستيراد بنجاح" + $" {count}/{assurSers.Count} ");
                }
                catch (Exception ex)
                {
                    await ex.AppLoggingAsync();
                }
            }
        });


        private static SerializableAssure AssureToSerializable(Assure assure)
        {
            var serializableAssure = new SerializableAssure
            {
                Nom = assure.Nom,
                Prenom = assure.Prenom,
                DateNaissance = assure.DateNaissanceString,
                ArPrenom = assure.ArPrenom,
                ArNom = assure.ArNom,
                Suspende = assure.Suspende,
                Matrecul = assure.Matricule,
                Adresse = assure.Adresse,
                Ccp = assure.GetCcp,
                Grade = assure.Grade,
                IsContractial = assure.IsContractial,
                Jusquea = assure.JusqueaString,
                Lieu = assure.Lieu,
                Note = assure.Note,
                Tel = assure.Tel
            };
            foreach (var patient in assure.Patients)
            {
                var p = new SerializablePatient
                {
                    Nom = patient.Nom,
                    Prenom = patient.Prenom,
                    DateNaissance = patient.DateNaissance?.ToString("dd-MM-yyyy"),
                    Suspende = patient.Suspende,
                    Alliance = patient.Alliance,
                    Au = patient.Au?.ToString("dd-MM-yyyy"),
                    Du = patient.Du?.ToString("dd-MM-yyyy")
                };
                serializableAssure.Patients.Add(p);
            }
            return serializableAssure;
        }
        private static Assure SerializableToAssure(SerializableAssure serAssure)
        {
            Assure newAssure = null;
            try
            {
                newAssure = new Assure
                {
                    Nom = serAssure.Nom,
                    Prenom = serAssure.Prenom,
                    DateNaissance = serAssure.DateNaissance.GetDateFromString(),
                    ArPrenom = serAssure.ArPrenom,
                    ArNom = serAssure.ArNom,
                    Suspende = serAssure.Suspende,
                    Matricule = serAssure.Matrecul,
                    Adresse = serAssure.Adresse,
                    Ccp = serAssure.Ccp,
                    Grade = serAssure.Grade,
                    IsContractial = serAssure.IsContractial,
                    Jusquea = serAssure.Jusquea.GetDateFromString(),
                    Lieu = serAssure.Lieu,
                    Note = serAssure.Note,
                    Tel = serAssure.Tel
                };
                foreach (var patient in serAssure.Patients)
                {
                    try
                    {
                        var p = new Patient
                        {
                            Nom = patient.Nom,
                            Prenom = patient.Prenom,
                            DateNaissance = patient.DateNaissance.GetDateFromString(),
                            Suspende = patient.Suspende,
                            Alliance = patient.Alliance,
                            Au = patient.Au.GetDateFromString(),
                            Du = patient.Du.GetDateFromString()
                        };
                        newAssure.Patients.Add(p);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return newAssure;
        }



        private async void ExportFormesBtn_OnClick(object sender, RoutedEventArgs e)
        {

            var save = new OpenFolderDialog();

            if (save.ShowDialog() == true)
            {
                var exportMedPath = save.FolderName + "\\Forms.xml";
                try
                {
                    var medArray = await Context.Formes.ToListAsync();
                    await Serialize.SerializeDataToXmlFileAsync(exportMedPath, medArray);
                    MessageBox.Show("تهانينا ", @"تم التصدير بنجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
