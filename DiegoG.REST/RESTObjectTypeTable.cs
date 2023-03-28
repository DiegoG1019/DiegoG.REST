namespace DiegoG.REST;

/// <summary>
/// Represents a table of Type information that informs the object type that represents a given <typeparamref name="TRESTObjectCode"/>
/// </summary>
/// <typeparam name="TRESTObjectCode">The type of code that will be used to discriminate object types</typeparam>
public abstract class RESTObjectTypeTable<TRESTObjectCode> where TRESTObjectCode : struct, IEquatable<TRESTObjectCode>
{
    /// <summary>
    /// Gets the <see cref="Type"/> description object for the object type represented by <paramref name="code"/>
    /// </summary>
    public abstract Type GetTypeFor(TRESTObjectCode code);

    /// <summary>
    /// Adds a relation between the type of <typeparamref name="TRESTObject"/> and <paramref name="code"/>
    /// </summary>
    public void AddTypeFor<TRESTObject>(TRESTObjectCode code) where TRESTObject : RESTObject<TRESTObjectCode>
    {
        AddType(typeof(TRESTObject), code);
    }

    /// <summary>
    /// Adds a relation between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    /// <remarks>
    /// If <paramref name="type"/> is not assignable to <see cref="RESTObject{TRESTObjectCode}"/>, an <see cref="ArgumentException"/> will be thrown
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    public void AddTypeFor(Type type, TRESTObjectCode code)
    {
        if (type.IsAssignableTo(typeof(RESTObject<TRESTObjectCode>)) is false)
            throw new ArgumentException($"type must be assignable to {typeof(RESTObject<TRESTObjectCode>)}", nameof(type));
        AddType(type, code);
    }

    /// <summary>
    /// Internally mutates this object to maintain the relation that is being added between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    protected abstract void AddType(Type type, TRESTObjectCode code);
}
