using OrdoMill.Interfaces;

namespace OrdoMill.Services;

public class ActionProgress<T> : IActionProgress<T>
{
    SynchronizationContext context;
    public ActionProgress()
    {
        context = SynchronizationContext.Current
                  ?? new SynchronizationContext();
    }

    public ActionProgress(Action<T> action)
        : this()
    {
        ProgressReported += action;
    }

    public event Action<T> ProgressReported;

    private T value;

    public T Value
    {
        get { return value; }
        set
        {
            this.value = value;
            ((IActionProgress<T>)this).Report(this.value);
        }
    }

    void IActionProgress<T>.Report(T data)
    {
        var action = ProgressReported;
        if (action != null)
        {
            Thread.Sleep(100);
            value = data;
            context.Post(arg => action((T)arg), data);
        }
    }
}