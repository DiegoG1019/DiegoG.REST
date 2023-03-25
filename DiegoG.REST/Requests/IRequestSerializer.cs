using System.Collections.Immutable;

namespace DiegoG.RESTBase.Requests;

/// <summary>
/// Represents an object capable of serializing and deserializing a request
/// </summary>
/// <typeparam name="TRequestCode">The type of code that will be used to discriminate requests</typeparam>
public interface IRequestSerializer<TRequestCode> where TRequestCode : struct, IEquatable<TRequestCode>
{
    /// <summary>
    /// The MIME types that this serializes emits and receives
    /// </summary>
    public ImmutableArray<string> MIMETypes { get; }

    /// <summary>
    /// Serializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <param name="request">The request object that is to be serialized</param>
    /// <param name="output">The stream in which the serialized object will be written</param>
    public void Serialize(RESTRequestBase<TRequestCode> request, Stream output);

    /// <summary>
    /// Serializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <param name="request">The request object that is to be serialized</param>
    /// <param name="output">The stream in which the serialized object will be written</param>
    public Task SerializeAsync(RESTRequestBase<TRequestCode> request, Stream output);

    /// <summary>
    /// Verifies if the mime types in <paramref name="mimeTypes"/> are also contained in <see cref="MIMETypes"/>
    /// </summary>
    /// <returns><see langword="true"/> if at least one of the types in <paramref name="mimeTypes"/> is contained in <see cref="MIMETypes"/> and/or are otherwised considered valid by this object</returns>
    public bool VerifyTypes(IEnumerable<string> mimeTypes)
    {
        foreach (var i in mimeTypes)
            if (MIMETypes.Contains(i))
                return true;
        return false;
    }

    /// <summary>
    /// Deserializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <param name="stream">The stream to read the request from</param>
    /// <param name="table">The table from which to read the type through the code</param>
    public RESTRequestBase<TRequestCode> Deserialize(Stream stream, RequestTypeTable<TRequestCode> table);

    /// <summary>
    /// Deserializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <typeparam name="TRequest">The base type, or type of request that is expected to be deserialized from the stream</typeparam>
    /// <param name="stream">The stream to read the request from</param>
    /// <param name="table">The table from which to read the type through the code</param>
    /// <exception cref="ArgumentException"></exception>
    public TRequest Deserialize<TRequest>(Stream stream, RequestTypeTable<TRequestCode> table) where TRequest : RESTRequestBase<TRequestCode>
    {
        var dat = Deserialize(stream, table);
        return dat as TRequest ?? throw new ArgumentException($"The request could not be deserialized into a request of type {typeof(TRequest)}", nameof(stream));
    }

    /// <summary>
    /// Deserializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <param name="stream">The stream to read the request from</param>
    /// <param name="table">The table from which to read the type through the code</param>
    public Task<RESTRequestBase<TRequestCode>> DeserializeAsync(Stream stream, RequestTypeTable<TRequestCode> table);

    /// <summary>
    /// Deserializes a given RESTRequest into the format used by this object
    /// </summary>
    /// <typeparam name="TRequest">The base type, or type of request that is expected to be deserialized from the stream</typeparam>
    /// <param name="stream">The stream to read the request from</param>
    /// <param name="table">The table from which to read the type through the code</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task<TRequest> DeserializeAsync<TRequest>(Stream stream, RequestTypeTable<TRequestCode> table) where TRequest : RESTRequestBase<TRequestCode>
    {
        var dat = await DeserializeAsync(stream, table).ConfigureAwait(false);
        return dat as TRequest ?? throw new ArgumentException($"The request could not be deserialized into a request of type {typeof(TRequest)}", nameof(stream));
    }
}