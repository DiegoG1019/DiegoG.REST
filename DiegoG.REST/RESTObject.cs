namespace DiegoG.REST;

/// <summary>
/// The base of all rest objects
/// </summary>
/// <typeparam name="TObjectCode">The type of code that will be used to discriminate objects</typeparam>
public abstract class RESTObject<TObjectCode> where TObjectCode : struct, IEquatable<TObjectCode>
{
    /// <summary>
    /// The unique code that represents this object
    /// </summary>
    public TObjectCode Code { get; }

    /// <summary>
    /// Initializes this REST Object with the respective ObjectCode
    /// </summary>
    protected RESTObject(TObjectCode code)
    {
        Code = code;
    }

    /// <summary>
    /// This method should be called by <see cref="IRESTObjectSerializer{TRESTCode}"/> right before serialization starts, and pass this object's type as <paramref name="typeToSerialize"/>
    /// </summary>
    /// <param name="serializer">The serializer that started the serialization</param>
    /// <param name="typeToSerialize">The type that is being serialized, originally this object's type</param>
    public virtual void Serializing(IRESTObjectSerializer<TObjectCode> serializer, ref Type typeToSerialize) { }

    /// <summary>
    /// This method should be called by <see cref="IRESTObjectSerializer{TRESTCode}"/> right after deserialization ends
    /// </summary>
    /// <param name="serializer">The serializer that deserialized this object</param>
    public virtual void Deserialized(IRESTObjectSerializer<TObjectCode> serializer) { }
}
