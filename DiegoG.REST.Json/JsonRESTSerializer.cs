using System.Collections.Immutable;
using System.Text.Json;
using DiegoG.RESTBase.Requests;
using DiegoG.RESTBase.Responses;

namespace DiegoG.RESTBase.Json;
public class JsonRESTSerializer<TRESTCode> 
    : IRequestSerializer<TRESTCode>,
      IResponseSerializer<TRESTCode>
     where TRESTCode : struct, IEquatable<TRESTCode>
{
    public static ImmutableArray<string> JsonMIME { get; } = ImmutableArray.Create("application/json", "text/json");

    public JsonSerializerOptions? Options { get; init; }
    public ImmutableArray<string> MIMETypes => JsonMIME;

    public JsonRESTSerializer()  : this(null)
    {
    }

    public JsonRESTSerializer(JsonSerializerOptions? options)
    {
        Options = options;
    }

    #region Requests

    public void Serialize(RESTRequestBase<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        JsonSerializer.Serialize(output, request, type, Options);
    }

    public Task SerializeAsync(RESTRequestBase<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        return JsonSerializer.SerializeAsync(output, request, type, Options);
    }

    public RESTRequestBase<TRESTCode> Deserialize(Stream stream, RequestTypeTable<TRESTCode> table)
    {
        var doc = JsonSerializer.SerializeToDocument(stream, Options);
        var code = doc.RootElement.EnumerateObject().First(x => x.Name == "RequestCode").Value.Deserialize<TRESTCode>(Options);
        var type = table.GetTypeFor(code);
        return JsonSerializer.Deserialize(doc, type, Options) as RESTRequestBase<TRESTCode>
            ?? throw new InvalidDataException($"Could not deserialize an object of type {type} from the stream");
    }

    public async Task<RESTRequestBase<TRESTCode>> DeserializeAsync(Stream stream, RequestTypeTable<TRESTCode> table)
    {
        await Task.Yield();
        return Deserialize(stream, table);
    }

    #endregion

    #region Responses
    
    public void Serialize(RESTResponseBase<TRESTCode> response, Stream output)
    {
        var type = response.GetType();
        response.Serializing(this, ref type);
        JsonSerializer.Serialize(output, response, type, Options);
    }

    public Task SerializeAsync(RESTResponseBase<TRESTCode> response, Stream output)
    {
        var type = response.GetType();
        response.Serializing(this, ref type);
        return JsonSerializer.SerializeAsync(output, response, type, Options);
    }

    public RESTResponseBase<TRESTCode> Deserialize(Stream stream, ResponseTypeTable<TRESTCode> table)
    {
        var doc = JsonSerializer.SerializeToDocument(stream, Options);
        var code = doc.RootElement.EnumerateObject().First(x => x.Name == "ResponseCode").Value.Deserialize<TRESTCode>(Options);
        var type = table.GetTypeFor(code);
        return JsonSerializer.Deserialize(doc, type, Options) as RESTResponseBase<TRESTCode>
            ?? throw new InvalidDataException($"Could not deserialize an object of type {type} from the stream");
    }

    public async Task<RESTResponseBase<TRESTCode>> DeserializeAsync(Stream stream, ResponseTypeTable<TRESTCode> table)
    {
        await Task.Yield();
        return Deserialize(stream, table);
    }

    #endregion
}
