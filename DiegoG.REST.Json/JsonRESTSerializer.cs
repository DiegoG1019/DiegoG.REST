using System.Collections.Immutable;
using System.Text.Json;
using DiegoG.REST;

namespace DiegoG.REST.Json;

/// <summary>
/// A serializer that serializes and deserializes <see cref="RESTObject{TObjectCode}"/>s
/// </summary>
public class JsonRESTSerializer<TRESTCode>
    : IRESTObjectSerializer<TRESTCode>
     where TRESTCode : struct, IEquatable<TRESTCode>
{
    /// <summary>
    /// Common Json MIME types
    /// </summary>
    public static ImmutableArray<string> JsonMIME { get; } = ImmutableArray.Create("application/json", "text/json");

    /// <inheritdoc/>
    public string Charset { get; } = "utf-8";

    /// <summary>
    /// The JsonSerializerOptions used for this serializer
    /// </summary>
    public JsonSerializerOptions? Options { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<string> MIMETypes => JsonMIME;

    /// <summary>
    /// Instances a new object of type <see cref="JsonRESTSerializer{TRESTCode}"/>
    /// </summary>
    public JsonRESTSerializer(JsonSerializerOptions? options = null)
    {
        Options = options;
    }

    /// <inheritdoc/>
    public void Serialize(RESTObject<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        JsonSerializer.Serialize(output, request, type, Options);
    }

    /// <inheritdoc/>
    public Task SerializeAsync(RESTObject<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        return JsonSerializer.SerializeAsync(output, request, type, Options);
    }

    /// <inheritdoc/>
    public RESTObject<TRESTCode> Deserialize(Stream stream, RESTObjectTypeTable<TRESTCode> table)
    {
        var doc = JsonSerializer.SerializeToDocument(stream, Options);
        var code = doc.RootElement.EnumerateObject().First(x => x.Name == "RequestCode").Value.Deserialize<TRESTCode>(Options);
        var type = table.GetTypeFor(code);
        return doc.Deserialize(type, Options) as RESTObject<TRESTCode>
            ?? throw new InvalidDataException($"Could not deserialize an object of type {type} from the stream");
    }

    /// <inheritdoc/>
    public async Task<RESTObject<TRESTCode>> DeserializeAsync(Stream stream, RESTObjectTypeTable<TRESTCode> table)
    {
        await Task.Yield();
        return Deserialize(stream, table);
    }
}
