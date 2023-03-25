namespace DiegoG.RESTBase.Responses;

/// <summary>
/// The base of all rest requests
/// </summary>
/// <typeparam name="TResponseCode">The type of code that will be used to discriminate requests</typeparam>
public abstract class RESTResponseBase<TResponseCode> where TResponseCode : struct, IEquatable<TResponseCode>
{
    /// <summary>
    /// The unique code that represents this object
    /// </summary>
    public TResponseCode ResponseCode { get; }

    /// <summary>
    /// This method should be called by <see cref="IResponseSerializer{TRESTCode}"/> right before serialization starts, and pass this object's type as <paramref name="typeToSerialize"/>
    /// </summary>
    /// <param name="serializer">The serializer that started the serialization</param>
    /// <param name="typeToSerialize">The type that is being serialized, originally this object's type</param>
    public virtual void Serializing(IResponseSerializer<TResponseCode> serializer, ref Type typeToSerialize) { }

    /// <summary>
    /// This method should be called by <see cref="IResponseSerializer{TRESTCode}"/> right after deserialization ends
    /// </summary>
    /// <param name="serializer">The serializer that deserialized this object</param>
    public virtual void Deserialized(IResponseSerializer<TResponseCode> serializer) { }
}
