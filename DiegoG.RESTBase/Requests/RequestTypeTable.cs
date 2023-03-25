namespace DiegoG.RESTBase.Requests;

/// <summary>
/// Represents a table of Type information that informs the request type that represents a given <typeparamref name="TRequestCode"/>
/// </summary>
/// <typeparam name="TRequestCode">The type of code that will be used to discriminate request types</typeparam>
public abstract class RequestTypeTable<TRequestCode> where TRequestCode : struct, IEquatable<TRequestCode>
{
    /// <summary>
    /// Gets the <see cref="Type"/> description object for the request type represented by <paramref name="code"/>
    /// </summary>
    public abstract Type GetTypeFor(TRequestCode code);

    /// <summary>
    /// Adds a relation between the type of <typeparamref name="TRequest"/> and <paramref name="code"/>
    /// </summary>
    public void AddTypeFor<TRequest>(TRequestCode code) where TRequest : RESTRequestBase<TRequestCode>
    {
        AddType(typeof(TRequest), code);
    }

    /// <summary>
    /// Adds a relation between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    /// <remarks>
    /// If <paramref name="type"/> is not assignable to <see cref="RESTRequestBase{TRequestCode}"/>, an <see cref="ArgumentException"/> will be thrown
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    public void AddTypeFor(Type type, TRequestCode code)
    {
        if (type.IsAssignableTo(typeof(RESTRequestBase<TRequestCode>)) is false)
            throw new ArgumentException($"type must be assignable to {typeof(RESTRequestBase<TRequestCode>)}", nameof(type));
        AddType(type, code);
    }

    /// <summary>
    /// Internally mutates this object to maintain the relation that is being added between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    protected abstract void AddType(Type type, TRequestCode code);
}
