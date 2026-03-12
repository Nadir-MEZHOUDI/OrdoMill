using OrdoMill.Properties;
using OrdoMill.Services;
using OrdoMill.ViewModel;
using PropertyChanged;

namespace OrdoMill.Views.Home;

[AddINotifyPropertyChangedInterface]
public class LicenseViewModel : DialogContentViewModel
{
    const string SuccessMessage = "ÔßÑÇ ..Êã ÇáÊÓÌíá ÈäÌÇÍ ...";
    //  const string EnterSerialMessage = @"ÝÖáÇ Þã ÈÅÏÎÇá ÇáÑÞã ÇáÊÓáÓáí ááãäÊÌ";
    const string ErrorMessage = "ÇáÑÞã ÇáÐí ÃÏÎáÊå ÎÇØÆ åá ÊÑíÏ ÅÚÇÏÉ ÇáãÍÇæáÉ?";
    public string OutputMessage { get; set; }
    public string FinalKey { get; set; }
    public string PrimaryKey => Encryptor.GetPrimaryKey();
   public override bool CheckHideDialogesConditions()
    {
        Settings.Default.Key = FinalKey;
        Settings.Default.Save();
        if (Encryptor.CompareKey(Settings.Default.Key))
        {
            OutputMessage = SuccessMessage;
            return true;
        }
        OutputMessage = ErrorMessage;
        return false;
    }
    public override bool CheckShowDialogesConditions()
    {
        return !Encryptor.CompareKey(Settings.Default.Key);
    }
}