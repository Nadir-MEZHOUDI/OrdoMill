using MahApps.Metro.Controls.Dialogs;
using OrdoMill.Data.Model;
using OrdoMill.Properties;
using OrdoMill.ViewModel;
using System.Windows;

namespace OrdoMill.Services;

public static class Statics
{
    public static MetroDialogSettings ErrorMessageSettings => new LoginDialogSettings
    {
        DefaultButtonFocus = MessageDialogResult.Affirmative,
        AffirmativeButtonText = "Yes".GetString(),
        NegativeButtonText = "No".GetString(),
        ColorScheme = MetroDialogColorScheme.Accented
    };

    public static ViewModelLocator Locator
        => Application.Current.Resources["Locator"] as ViewModelLocator;

    public static MetroDialogSettings MessageSettings => new LoginDialogSettings
    {
        DialogMessageFontSize = 15,
        DialogTitleFontSize = 17,
        DefaultButtonFocus = MessageDialogResult.Affirmative,
        AffirmativeButtonText = "Yes".GetString(),
        NegativeButtonText = "No".GetString()
    };

    internal static async void SaveBackUp()
    {
        try
        {
            //TODO: add a progress dialog
            //TODO: add a setting to choose the backup path
            Settings.Default.LastSaveDate = DateTime.Now.Date;
            Settings.Default.Save();
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync();
        }
    }

    public static void AutoBackUp()
    {
        switch (Settings.Default.SaveMode)
        {
            case "لا شيء":
                break;

            case "يوميا":
                SaveBackUp();
                break;

            case "أسبوعيا":
                if ((Now() - Settings.Default.LastSaveDate).TotalDays >= 7)
                    SaveBackUp();
                break;

            case "شهريا":
                if ((Now() - Settings.Default.LastSaveDate).TotalDays >= 30)
                    SaveBackUp();
                break;

            case "سنويا":
                if ((Now() - Settings.Default.LastSaveDate).TotalDays >= 364.25)
                    SaveBackUp();
                break;
        }
    }

    public static async Task CreateHistoAsync(OpNames operationId, string note = "", int? patientId = null)
    {
        try
        {
            using var context = new DbCon(Settings.Default.ConnectionString);
            var histo = new Historique
            {
                UserId = Locator.Main.LoginDialog.ViewModel?.LoggedUser?.Id ?? 1,
                OperationId = (int)operationId,
                DateTime = Now(),
                Note = note,
                PatientId = patientId
            };
            context.Historiques?.Add(histo);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await ex.AppLoggingAsync(send: false);
        }
    }
           

    public static string GetCopyName(string prefix = null)
    {
        try
        {
            if (prefix == null) prefix = Settings.Default.dbName;
            return prefix + DateTime.Now.ToString("yyyyMMdd");
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static DateTime Now() => DateTime.Now.AddDays(Settings.Default.subDays);

    public static IQueryable<T> PagedResult<T>(this IQueryable<T> query, int rowsCount, int pageNum = 1,
        int pageSize = 0)
    {
        if (query == null) return null;
        if (pageSize <= 0) pageSize = Settings.Default.PageSize;
        //If page number should be > 0 else set to first page
        if ((rowsCount <= pageSize) || (pageNum <= 0)) pageNum = 1;
        //Calculate number of rows to skip on page size
        var excludedRows = (pageNum - 1) * pageSize;
        // query = query.OrderBy(orderByProperty);
        //Skip the required rows for the current page and take the next records of pagesize count
        return query?.Skip(excludedRows)?.Take(pageSize);
    }


    //public static async Task ShowMessage(this IDialogCoordinator dialogCoordinator, object context,
    //    string message = "��", int millisecondsDelay = 1000, int fontSize = 20)
    //{
    //    var dialog = new CustomDialog { Content = message, FontSize = fontSize };
    //    await dialogCoordinator.ShowMetroDialogAsync(context, dialog);
    //    await Task.Delay(millisecondsDelay);
    //    await dialogCoordinator.HideMetroDialogAsync(context, dialog);

    //}
}
