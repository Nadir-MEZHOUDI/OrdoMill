namespace OrdoMill.Interfaces;

public interface IActionProgress<T>
{
    void Report(T data);
    T Value { get; set; }
}