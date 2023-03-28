using System.Collections.Immutable;
using System.Text.Json;
using DiegoG.REST;

namespace DiegoG.REST.Json;
public class JsonRESTSerializer<TRESTCode>
    : IRESTObjectSerializer<TRESTCode>
     where TRESTCode : struct, IEquatable<TRESTCode>
{
    public static ImmutableArray<string> JsonMIME { get; } = ImmutableArray.Create("application/json", "text/json");

    public string Charset { get; } = "utf-8";

    public JsonSerializerOptions? Options { get; init; }
    public ImmutableArray<string> MIMETypes => JsonMIME;

    public JsonRESTSerializer() : this(null)
    {
    }

    public JsonRESTSerializer(JsonSerializerOptions? options)
    {
        Options = options;
    }

    public void Serialize(RESTObject<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        JsonSerializer.Serialize(output, request, type, Options);
    }

    public Task SerializeAsync(RESTObject<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        return JsonSerializer.SerializeAsync(output, request, type, Options);
    }

    public RESTObject<TRESTCode> Deserialize(Stream stream, RESTObjectTypeTable<TRESTCode> table)
    {
        var doc = JsonSerializer.SerializeToDocument(stream, Options);
        var code = doc.RootElement.EnumerateObject().First(x => x.Name == "RequestCode").Value.Deserialize<TRESTCode>(Options);
        var type = table.GetTypeFor(code);
        return doc.Deserialize(type, Options) as RESTObject<TRESTCode>
            ?? throw new InvalidDataException($"Could not deserialize an object of type {type} from the stream");
    }

    public async Task<RESTObject<TRESTCode>> DeserializeAsync(Stream stream, RESTObjectTypeTable<TRESTCode> table)
    {
        await Task.Yield();
        return Deserialize(stream, table);
    }
}
