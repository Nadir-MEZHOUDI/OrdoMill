using System.Printing;
using System.Windows;
using System.Windows.Controls;
using OrdoMill.Properties;
using OrdoMill.Services;
using static OrdoMill.Helpers.Helper;

namespace OrdoMill.ViewModel;

public static class PrintHelper
{
    public static void QPrint(Grid can, int n = 1)
    {
        Print(false, can, n);
    }

    public static void Print(Grid can, int n = 1)
    {
        Print(true, can, n);
    }

    public static async void Print(bool showDialog, Grid can, int copiesNumber = 1)
    {
        //  ⁄ÌÌ‰ «··”«‰ «·„Õœœ Ê«·⁄‰’— «·–Ì ÌÕ ÊÌ ⁄·Ï «·ÊÀÌﬁ…

        // €ÌÌ— √·Ê«‰ √œ«… «·‰’ ≈·Ï ‘›«›
        foreach (var txtBox in FindVisualChildren<TextBox>(can))
            txtBox.Style = Application.Current.Resources["TransparentText"] as Style;

        var printDlg = new PrintDialog { PrintTicket = {CopyCount = copiesNumber } };
        try
        {
            //Ã·» «”„ «·ÿ«»⁄… „‰ «·„Õ›ÊŸ« 
            if (!string.IsNullOrEmpty(Settings.Default.printerName))
                using (var prntServer = new PrintServer())
                    printDlg.PrintQueue = new PrintQueue(prntServer, Settings.Default.printerName);
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
            // ignored
        }

        // ⁄ÌÌ‰ ≈⁄œ«œ«  «·ÿ»«⁄… ÊÕÃ„ «·Ê—ﬁ

        using (var lps = new LocalPrintServer())
        {
            var defaultPrintQueue = lps.DefaultPrintQueue;
            var defaultPrintTicket = defaultPrintQueue.DefaultPrintTicket;
            defaultPrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA5);
            printDlg.PrintTicket = defaultPrintTicket;
            printDlg.PrintTicket.CopyCount = copiesNumber;
        }

        //Size pageSize = new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight);

        //// sizing of the element.
        //can.Measure(pageSize);
        //can.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));

        //≈÷Â«— ‰«›–… «·ÕÊ«— √Ê·«
        if (showDialog)
        {
            if (printDlg.ShowDialog() == true)
            {
                //ÿ»«⁄…
                printDlg.PrintVisual(can, "Imprimer les information ");

                //Õ›Ÿ «”„ «·ÿ«»⁄…
                Settings.Default.printerName = printDlg.PrintQueue.Name;
                Settings.Default.Save();
            }
        }
        else
        {
            var m = new Window();
            m.Show();
            m.Focus();
            m.Close();
            //ÿ»«⁄…
            printDlg.PrintVisual(can, "Imprimer les information ");
        }

        // €ÌÌ— ·Ê‰ «·Œ·›Ì… ··‰’Ê’ ≈·Ï „—∆Ì
        foreach (var txtBox in FindVisualChildren<TextBox>(can))
            txtBox.Style = Application.Current.Resources["GreenText"] as Style;
    }
}