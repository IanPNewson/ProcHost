namespace ProcHost.Model;

public class ConstructorParam
{
    public string Name { get; set; }

    public object? Value { get; set; }
}

public abstract class ConstructorParam<T> : ConstructorParam
{
    public new T? Value { get; set; }
}
