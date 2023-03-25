namespace DiegoG.RESTBase.Responses;

/// <summary>
/// Represents a table of Type information that informs the request type that represents a given <typeparamref name="TResponseCode"/>
/// </summary>
/// <typeparam name="TResponseCode">The type of code that will be used to discriminate request types</typeparam>
public abstract class ResponseTypeTable<TResponseCode> where TResponseCode : struct, IEquatable<TResponseCode>
{
    /// <summary>
    /// Gets the <see cref="Type"/> description object for the request type represented by <paramref name="code"/>
    /// </summary>
    public abstract Type GetTypeFor(TResponseCode code);

    /// <summary>
    /// Adds a relation between the type of <typeparamref name="TResponse"/> and <paramref name="code"/>
    /// </summary>
    public void AddTypeFor<TResponse>(TResponseCode code) where TResponse : RESTResponseBase<TResponseCode>
    {
        AddType(typeof(TResponse), code);
    }

    /// <summary>
    /// Adds a relation between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    /// <remarks>
    /// If <paramref name="type"/> is not assignable to <see cref="RESTResponseBase{TResponseCode}"/>, an <see cref="ArgumentException"/> will be thrown
    /// </remarks>
    /// <exception cref="ArgumentException"></exception>
    public void AddTypeFor(Type type, TResponseCode code)
    {
        if (type.IsAssignableTo(typeof(RESTResponseBase<TResponseCode>)) is false)
            throw new ArgumentException($"type must be assignable to {typeof(RESTResponseBase<TResponseCode>)}", nameof(type));
        AddType(type, code);
    }

    /// <summary>
    /// Internally mutates this object to maintain the relation that is being added between <paramref name="type"/> and <paramref name="code"/>
    /// </summary>
    protected abstract void AddType(Type type, TResponseCode code);
}
