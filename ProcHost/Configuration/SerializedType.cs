namespace ProcHost.Model;

public class SerializedType
{
    public Type Type { get; set; }

    public List<ConstructorParam> ConstructorParams { get; set; }

    public T Instantiate<T>()
    {
        var constructors = Type.GetConstructors()
            .OrderByDescending(ctor => ctor.GetParameters().Length);
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            if (parameters.All(p => ConstructorParams.Select(cp => cp.Name).Contains(p.Name)))
            {
                var ctorArgs = parameters.Select(p => ConstructorParams.First(cp => cp.Name == p.Name).Value).ToArray();

                var instance = constructor.Invoke(ctorArgs);
                return (T)instance;
            }
        }

        throw new InvalidOperationException($"Could not instantiate type {Type.FullName} as all necessary constructor parameters were not found among '{string.Join(", ", ConstructorParams.Select(cp => cp.Name))}'");
    }
}
