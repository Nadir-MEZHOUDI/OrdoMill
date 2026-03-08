using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MethodTimer;
using OfficeOpenXml;
using OrdoMill.Data.Model;
using OrdoMill.Interfaces;
using SmartApp.Helpers.Helpers;

namespace OrdoMill.Services
{
    public static class ExtractionMethods
    {
        private const string FactureTemplateWith50Ordonnances = @"Templates\T1.xlsx";
        private const string FactureTemplateWithMorThen25Ordonnances = @"Templates\T3.xlsx";
        private const string FactureTemplateWithLessThen25Ordonnances = @"Templates\T2.xlsx";
        private static readonly CultureInfo Culture = new CultureInfo("fr-FR");

        public static async Task ExtractBordereauEtiquette(this Bordereau bordereau, string savePath, Info info)
        {
            try
            {
                await CheckTemplates();
                //create new XLS file
                var template = new FileInfo(@"Templates\E1.xlsx");
                var file =
                    new FileInfo(
                        $@"{savePath}\Etiquettes {bordereau.Date?.ToString("MMMM - yyyy", Culture)
                            .ToUpper()
                            .ToValidFileName()}.xlsx");
                var package = new ExcelPackage(file, template);

                #region Worksheet1

                var ws1 = package.Workbook.Worksheets[1];
                ws1.Cells["A3"].Value = info?.Pharmacie;
                ws1.Cells["A4"].Value =
                    $"Fact Du {bordereau.Factures.Min(x => x.Number)} Au {bordereau.Factures.Max(x => x.Number)}";
                ws1.Cells["A5"].Value = $"{bordereau.OrdosCount} ORDS";
                ws1.Cells["A6"].Value = bordereau.Date?.ToString("MMMM - yyyy").ToUpper();
                // ws1.Cells["A7"].Value = "'1/1'";

                #endregion Worksheet1

                package.Workbook.Properties.Title = $"Bordereau {info?.Pharmacie} {DateTime.Now.Date}";

                package.Workbook.Properties.Author = string.Join(Environment.NewLine, "NadirElghazali@gmail.com",
                    "0666346066", "Mezhoudi Hadj Nadir", "OrdoMill");

                package.Workbook.Properties.Company = "SmartApp / OrdoMill / Mezhoudi Hadj Nadir";

                package.Save();
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        public static async Task ExtractBordereauResume(this Bordereau bordereau, string savePath, Info info)
        {
            try
            {
                await CheckTemplates();

                //create new XLS file
                var template = new FileInfo(@"Templates\R1.xlsx");
                var file =
                    new FileInfo(
                        $@"{savePath}\Bordereau d'envoi {bordereau.Date?.ToString("MMMM - yyyy", Culture)
                            .ToUpper()
                            .ToValidFileName()}.xlsx".ToValidPath());
                var package = new ExcelPackage(file, template);

                #region Worksheet1

                var ws1 = package.Workbook.Worksheets[1];

                ws1.Cells["D1"].Value = $"{info.Region} Le:";
                ws1.Cells["D1"].AutoFitColumns();

                ws1.Cells["E1"].Value = (bordereau.Date ?? DateTime.Now).ToString("dd/MM/yyyy", Culture);

                ws1.Cells["C1"].Value = info.Pharmacie;

                ws1.Cells["C2"].Value = info.Adresse;
                ws1.Cells["C3"].Value = $"{info.Tel}/{info.Fax}";
                ws1.Cells["C4"].Value = info.CodeFiscal;
                ws1.Cells["C5"].Value = info.Article;
                ws1.Cells["C6"].Value = info.Rc;
                ws1.Cells["C7"].Value = info.Ccp;
                // ws1.Cells["E2"].Value = bordereau.Number;

                ws1.Cells["D3"].Value = $"Doit: {info.Doit}";

                ws1.Cells["A9"].Value =
                    $"BORDEREAU D'ENVOI: {(bordereau.Date ?? DateTime.Now).ToString("MMMM yyyy", Culture)}".ToUpper();

                var row = 12;
                var I = 1;
                var facturs = bordereau.Factures.OrderBy(facture => facture.Number);
                foreach (var facture in facturs)
                {
                    if (row > 12) ws1.InsertRow(row, 1);
                    ws1.Cells["A" + row].Value = I.ToString();
                    ws1.Cells["A" + row].StyleID = ws1.Cells["A" + 12].StyleID;

                    ws1.Cells[row, 2, row, 3].Merge = true;
                    ws1.Cells["B" + row].Value = facture.Name;
                    ws1.Cells["B" + row].StyleID = ws1.Cells["B" + 12].StyleID;
                    ws1.Cells["C" + row].StyleID = ws1.Cells["B" + 12].StyleID;

                    ws1.Cells[row, 4, row, 5].Merge = true;
                    ws1.Cells["D" + row].Value = facture.Ordonnances.Count;
                    ws1.Cells["D" + row].StyleID = ws1.Cells["D" + 12].StyleID;
                    ws1.Cells["E" + row].StyleID = ws1.Cells["D" + 12].StyleID;

                    ws1.Cells["F" + row].Value = facture.Montant;
                    ws1.Cells["F" + row].StyleID = ws1.Cells["F" + 12].StyleID;
                    I++;
                    row++;
                }

                ws1.Cells["D" + row].Value = bordereau.OrdosCount;
                ws1.Cells["F" + row].Value = bordereau.Montant;

                #endregion Worksheet1

                package.Workbook.Properties.Title = $"Bordereau {info.Pharmacie} {DateTime.Now.Date}";
                package.Workbook.Properties.Author = string.Join(Environment.NewLine, "NadirElghazali@gmail.com",
                    "0666346066", "Mezhoudi Hadj Nadir", "OrdoMill");

                package.Workbook.Properties.Company = "SmartApp / OrdoMill / Mezhoudi Hadj Nadir";

                package.Save();

                //  Process.Start(file.FullName);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        }

        internal static bool ExtractFactureToExcel(this Facture facture, string savePath, Info info, IActionProgress<int> progress)
        {
            try
            {
                FileInfo template = null;

                Func<Facture, ExcelWorksheet, IActionProgress<int>, Task> action = null;

                progress.Value++;

                GetMethodByOrdonnancesCount(facture, ref template, ref action);
                progress.Value++;


                var newFilePath = new FileInfo($@"{savePath}\{facture.Name.ToValidFileName()}.xlsx");
                progress.Value += 5;


                var package = new ExcelPackage(newFilePath, template);
                var ws1 = package.Workbook.Worksheets[1];
                progress.Value += 5;


                action?.Invoke(facture, ws1, progress);
                progress.Value++;

                FillExcelFactureHeader(facture, info, package, progress);
                progress.Value++;

                FillExcelFactureDetails(facture, info, ref package, progress);
                progress.Value++;

                package.Save();

                progress.Report(100);
                return true;
            }
            catch (Exception ex)
            {
                ex.AppLogging();
                return false;
            }
        }

        private static void GetMethodByOrdonnancesCount(Facture facture, ref FileInfo template, ref Func<Facture, ExcelWorksheet, IActionProgress<int>, Task> action)
        {
            try
            {
                //TODO Add Password to Templates
                if (facture.Ordonnances?.Count >= 50)
                {
                    template = new FileInfo(FactureTemplateWith50Ordonnances);
                    action = FillExcelFactureWith50Ord;
                }
                else if (facture.Ordonnances?.Count <= 25)
                {
                    template = new FileInfo(FactureTemplateWithLessThen25Ordonnances);
                    action = FillExcelFactureLessThen25Ord;
                }
                else if (facture.Ordonnances?.Count > 25)
                {
                    template = new FileInfo(FactureTemplateWithMorThen25Ordonnances);
                    action = FillExcelFactureMoreThen25Ord;
                }
            }
            catch (Exception ex)
            {

            }
        }

        internal static async Task CheckTemplates() => await Task.Run(async () =>
        {
            try
            {
                if (!Directory.Exists("Templates"))
                    Directory.CreateDirectory("Templates");

                if (!File.Exists(FactureTemplateWith50Ordonnances))
                    File.WriteAllBytes(FactureTemplateWith50Ordonnances, GlobalResources.Properties.Resources.T1);

                if (!File.Exists(FactureTemplateWithLessThen25Ordonnances))
                    File.WriteAllBytes(FactureTemplateWithLessThen25Ordonnances, GlobalResources.Properties.Resources.T2);

                if (!File.Exists(FactureTemplateWithMorThen25Ordonnances))
                    File.WriteAllBytes(FactureTemplateWithMorThen25Ordonnances, GlobalResources.Properties.Resources.T3);
            }
            catch (Exception ex)
            {
                await ex.AppLoggingAsync();
            }
        });

        [Time]
        private static void FillExcelFactureDetails(Facture facture, Info info, ref ExcelPackage package, IActionProgress<int> progress)
        {
            try
            {
                if (package == null || facture == null) return;

                var ws2 = package.Workbook?.Worksheets[2];
                if (ws2 == null) return;
                ws2.Cells["B1"].Value = info?.Code + facture.MinName + (facture.Chronic ? "CH" : "") +
                                        (facture.Out ? facture.Region : "HR");
                ws2.Cells["H1"].Value = (facture.Bordereau?.Date ?? DateTime.Now).ToString("MMMM", Culture).ToUpper();
                ws2.Cells["I1"].Value = info?.Pharmacie;
                var j = 1;
                foreach (var ordo in facture.Ordonnances)
                {
                    ws2.Cells["A" + j].Value = ordo?.Patient?.Assure?.Matricule;
                    ws2.Cells["C" + j].Value = ordo?.Patient?.Assure?.GetCcp;
                    ws2.Cells["D" + j].Value = ordo?.Montant;
                    ws2.Cells["E" + j].Value = ordo?.Patient?.Prenom;
                    ws2.Cells["F" + j].Value = ordo?.Patient?.Alliance;
                    ws2.Cells["G" + j].Value = ordo?.SoineDate.ToString("dd-MM-yyyy", Culture);
                    var contenue = ordo?.Contenu;
                    ws2.Cells["J" + j].Value = contenue;
                    ws2.Cells["K" + j].Value = ordo?.Medecin?.FullName;
                    j++;
                    progress.Value++;
                }
                ws2.Column(11).AutoFit();
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        private static void FillExcelFactureHeader(Facture facture, Info info, ExcelPackage package, IActionProgress<int> progress)
        {
            try
            {
                progress.Value++;

                var ws1 = package.Workbook.Worksheets[1];
                ws1.Cells["C1"].Value = info.Pharmacie;
                ws1.Cells["C2"].Value = info.Adresse;
                ws1.Cells["C3"].Value = $"{info.Tel}/{info.Fax}";
                ws1.Cells["C4"].Value = info.CodeFiscal;
                ws1.Cells["C5"].Value = info.Article;
                ws1.Cells["C6"].Value = info.Rc;
                ws1.Cells["C7"].Value = info.Ccp;
                ws1.Cells["D1"].Value = $"{info.Region} LE:";
                ws1.Cells["E1"].Value = (facture.Bordereau?.Date ?? DateTime.Now).ToString("dd/MM/yyyy", Culture);
                ws1.Cells["D2"].Value = facture.Name;
                ws1.Cells["D3"].Value = $"Doit: {info.Doit}";
                progress.Value += 5;

                package.Workbook.Properties.Title = $"Bordereau {info.Pharmacie} {DateTime.Now.Date}";
                package.Workbook.Properties.Author = string.Join(Environment.NewLine, "NadirElghazali@gmail.com",
                    "0666346066", "Mezhoudi Hadj Nadir", "OrdoMill");
                progress.Value++;


                package.Workbook.Properties.Company = "Mezhoudi Hadj Nadir";
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }

        private static async Task FillExcelFactureWith50Ord(Facture facture, ExcelWorksheet ws1,
            IActionProgress<int> progress) => await Task.Run(() =>
        {
            try
            {
                double sousTotal = 0;
                var I = 10;
                var f = facture.Ordonnances.ToList();
                for (var i = 0; i < 25; i++)
                {
                    var ordo = f[i];
                    ws1.Cells["B" + I].Value = ordo?.Patient?.Assure?.Matricule;
                    ws1.Cells["C" + I].Value = ordo?.Patient?.Assure?.FullName;
                    ws1.Cells["D" + I].Value = ordo?.Patient?.Prenom;
                    ws1.Cells["E" + I].Value = ordo?.Montant?.ToString("F");
                    ws1.Cells["F" + I].Value = ordo?.Patient?.Assure?.GetCcp;
                    I++;
                    sousTotal += ordo?.Montant ?? 0;
                    progress.Value++;
                }
                ws1.Cells["E" + 35].Value = sousTotal;
                I = 39;
                for (var i = 25; i < 50; i++)
                {
                    var ordo = f[i];
                    ws1.Cells["B" + I].Value = ordo?.Patient?.Assure?.Matricule;
                    ws1.Cells["C" + I].Value = ordo?.Patient?.Assure?.FullName;
                    ws1.Cells["D" + I].Value = ordo?.Patient?.Prenom;
                    ws1.Cells["E" + I].Value = ordo?.Montant?.ToString("F");
                    ws1.Cells["F" + I].Value = ordo?.Patient?.Assure?.GetCcp;
                    I++;
                    progress.Value++;
                }
                ws1.Cells["A" + 64].Value = $"Total général de la {facture?.Name} : {facture?.Montant}";
                progress.Value++;


                ws1.Cells["A" + 68].Value = DecimalToArretation((decimal)(facture?.Montant ?? 0));
                progress.Value++;
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        });

        public static string DecimalToArretation(decimal montant)
        {
            try
            {
                var integerPart = Math.Truncate(montant);
                var stringIntegerPart = NbrToLtr.ConvertFr(integerPart)?.ToUpper();
                var fractionPart = (montant - Math.Truncate(montant)) * 100;
                var stringFractionPart = NbrToLtr.ConvertFr(fractionPart)?.ToUpper();
                return $"{stringIntegerPart} DINARS ET {stringFractionPart} CENTIMES";
            }
            catch (Exception ex)
            {
                ex.AppLogging();
                return string.Empty;
            }
        }

        private static Task FillExcelFactureLessThen25Ord(Facture facture, ExcelWorksheet ws1,
            IActionProgress<int> actionProgress) => Task.Run(() =>
        {
            try
            {
                var f = facture.Ordonnances.ToList();
                var row = 10;
                for (var i = 0; i < facture.Ordonnances.Count; i++)
                {
                    var ordo = f[i];
                    if (row > 10) ws1.InsertRow(row, 1);
                    ws1.Cells["A" + row].Value = (row - 9).ToString();
                    ws1.Cells["A" + row].StyleID = ws1.Cells["A" + 12].StyleID;

                    ws1.Cells["B" + row].Value = ordo?.Patient?.Assure?.Matricule;
                    ws1.Cells["B" + row].StyleID = ws1.Cells["B" + 10].StyleID;

                    ws1.Cells["C" + row].Value = ordo?.Patient?.Assure?.FullName;
                    ws1.Cells["C" + row].StyleID = ws1.Cells["C" + 10].StyleID;

                    ws1.Cells["D" + row].Value = ordo?.Patient?.Prenom;
                    ws1.Cells["D" + row].StyleID = ws1.Cells["D" + 10].StyleID;

                    ws1.Cells["E" + row].Value = ordo?.Montant?.ToString("F");
                    ws1.Cells["E" + row].StyleID = ws1.Cells["E" + 10].StyleID;

                    ws1.Cells["F" + row].Value = ordo?.Patient?.Assure?.GetCcp;
                    ws1.Cells["F" + row].StyleID = ws1.Cells["F" + 10].StyleID;
                    row++;
                    actionProgress?.Report(10 + i);
                }

                ws1.Cells["A" + row].Value = $"Total général de la {facture.Name} : {facture.Montant}";
                actionProgress?.Report(80);

                ws1.Cells["A" + (row + 4)].Value = DecimalToArretation((decimal)(facture.Montant ?? 0));
                actionProgress?.Report(85);

            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        });

        private static Task FillExcelFactureMoreThen25Ord(Facture facture, ExcelWorksheet ws1,
            IActionProgress<int> actionProgress) => Task.Run(() =>
        {
            try
            {
                double sousTotal = 0;
                var I = 10;
                for (var index = 0; index < 25; index++)
                {
                    var ordo = facture.Ordonnances.ToList()[index];
                    ws1.Cells["B" + I].Value = ordo?.Patient?.Assure?.Matricule;
                    ws1.Cells["C" + I].Value = ordo?.Patient?.Assure?.FullName;
                    ws1.Cells["D" + I].Value = ordo?.Patient?.Prenom;
                    ws1.Cells["E" + I].Value = ordo?.Montant?.ToString("F");
                    ws1.Cells["F" + I].Value = ordo?.Patient?.Assure?.GetCcp;
                    I++;
                    sousTotal += ordo?.Montant ?? 0;
                }
                ws1.Cells["E" + 35].Value = sousTotal;
                I = 39;
                var f = facture.Ordonnances.ToList();

                for (var index = 25; index < facture.Ordonnances.Count; index++)
                {
                    var ordo = f[index];
                    if (I > 39) ws1.InsertRow(I, 1);
                    ws1.Cells["A" + I].Value = (index + 1).ToString();
                    ws1.Cells["A" + I].StyleID = ws1.Cells["A" + 12].StyleID;

                    ws1.Cells["B" + I].Value = ordo?.Patient?.Assure?.Matricule;
                    ws1.Cells["B" + I].StyleID = ws1.Cells["B" + 10].StyleID;

                    ws1.Cells["C" + I].Value = ordo?.Patient?.Assure?.FullName;
                    ws1.Cells["C" + I].StyleID = ws1.Cells["C" + 10].StyleID;

                    ws1.Cells["D" + I].Value = ordo?.Patient?.Prenom;
                    ws1.Cells["D" + I].StyleID = ws1.Cells["D" + 10].StyleID;

                    ws1.Cells["E" + I].Value = ordo?.Montant?.ToString("F");
                    ws1.Cells["E" + I].StyleID = ws1.Cells["E" + 10].StyleID;

                    ws1.Cells["F" + I].Value = ordo?.Patient?.Assure?.GetCcp;
                    ws1.Cells["F" + I].StyleID = ws1.Cells["F" + 10].StyleID;
                    I++;
                    actionProgress?.Report(15 + index);
                }

                ws1.Cells["A" + I].Value = $"Total général de la {facture.Name} : {facture.Montant}";
                actionProgress?.Report(80);

                ws1.Cells["A" + (I + 4)].Value = DecimalToArretation((decimal)(facture.Montant ?? 0));
                actionProgress?.Report(85);
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        });

        public static async Task ExtractAllBordereauFactures(this Bordereau bordereau, string savePath, Info info,
                IActionProgress<int> progressGlobal = null, IActionProgress<int> progressDetails = null)

        {
            try
            {
                if (bordereau == null)
                    return;
                var facturesList = bordereau.Factures.ToList();
                var factursCount = facturesList.Count;

                for (var i = 0; i < factursCount; i++)
                {
                    var facture = facturesList[i];
                    await Task.Run(() => facture.ExtractFactureToExcel(savePath, info, progressDetails));
                    progressGlobal?.Report(i * factursCount / 100);
                }
            }
            catch (Exception ex)
            {
                ex.AppLogging();
            }
        }
    }
}