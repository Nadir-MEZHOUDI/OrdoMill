using System.Diagnostics;

namespace OrdoMill.Helpers;

[Serializable]
public class SerializableError
{
    public DateTime TimeStamp { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public string TargetSite { get; set; }

    public string Caller { get; set; }

    public SerializableError InnerException { get; set; }

    public override string ToString() =>
        $"Message :{Message};\n StackTrace:{StackTrace};\n TimeStamp:{TimeStamp};\n InnerException {InnerException?.ToString() ?? "Null"}";
}

public class ExceptionsLogger
{
    private readonly EmailSender sender;

    public ExceptionsLogger(string filePath, EmailSender sender)
    {
        FilePath = filePath;
        this.sender = sender;
    }

    public ExceptionsLogger(EmailSender sender)
    {
        this.sender = sender;
    }

    public ExceptionsLogger(string filePath)
    {
        FilePath = filePath;
    }

    public string SpecifiedName { get; set; } = "OrdoMill App";

    public string Sender { get; set; } = Environment.MachineName;

    private string FilePath { get; set; }

    private SerializableError ConvertException(Exception ex)
    {
        return new SerializableError
        {
            Message = ex?.Message ?? string.Empty,
            StackTrace = ex?.StackTrace ?? string.Empty,
            TargetSite = ex?.TargetSite?.ToString(),
            Caller = ex != null ? new StackTrace(ex).GetFrames()?.Aggregate(string.Empty, (current, stackFrame) => current + stackFrame.GetMethod().Name + "◄-") : "Unknown",
            TimeStamp = DateTime.Now,
            InnerException = ex?.InnerException != null ? ConvertException(ex.InnerException) : null
        };
    }

    public void SerializeException(Exception ex)
    {
        if (FilePath == null)
        {
            return;
        }

        SerializableError error = ConvertException(ex);
        Serialize.SerializeDataToXmlFile(FilePath, error, true);
    }

    public async Task SerializeExceptionAsync(Exception ex)
    {
        if (FilePath == null)
        {
            return;
        }

        SerializableError error = ConvertException(ex);
        await Serialize.SerializeDataToXmlFileAsync(FilePath, error, true);
    }

    public void SendExceptionToMail(Exception ex)
    {
        try
        {
            if (sender == null)
            {
                return;
            }

            SerializableError error = ConvertException(ex);
            sender.Send("Exception From " + SpecifiedName, error.ToString(), Sender);
        }
        catch (Exception)
        {
        }
    }

    public async Task<bool> SendExceptionToMailAsync(Exception ex)
    {
        try
        {
            if (sender == null)
            {
                return false;
            }

            SerializableError error = ConvertException(ex);
            return await sender.SendAsync("Exception From " + SpecifiedName, error.ToString(), Sender);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
