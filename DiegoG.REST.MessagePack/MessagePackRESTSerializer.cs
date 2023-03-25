using System.Buffers;
using System.Collections.Immutable;
using DiegoG.RESTBase.Requests;
using MessagePack;

namespace DiegoG.RESTBase.MessagePack;

public class MessagePackRESTSerializer<TRESTCode> 
    : IRequestSerializer<TRESTCode>
    where TRESTCode : unmanaged, Enum, IEquatable<TRESTCode>
{
    private readonly struct RequestCodeBuffer
    {
        public TRESTCode RequestCode { get; init; }
    }

    public MessagePackSerializerOptions? Options { get; }

    public MessagePackRESTSerializer(MessagePackSerializerOptions? options, ImmutableArray<string> mIMETypes)
    {
        //Options = options ?? new MessagePackSerializerOptions(new RESTFormatterResolver());
        MIMETypes = mIMETypes;
    }

    public ImmutableArray<string> MIMETypes { get; }

    public void Serialize(RESTRequestBase<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        MessagePackSerializer.Serialize(type, output, request, Options);
    }

    public Task SerializeAsync(RESTRequestBase<TRESTCode> request, Stream output)
    {
        var type = request.GetType();
        request.Serializing(this, ref type);
        return MessagePackSerializer.SerializeAsync(type, output, request, Options);
    }

    public RESTRequestBase<TRESTCode> Deserialize(Stream stream, RequestTypeTable<TRESTCode> table)
    {
        if (stream.Length > int.MaxValue)
            throw new ArgumentException("The stream is too large", nameof(stream));

        byte[]? rented = null;
        try
        {
            if (stream.CanSeek is false)
            {
                rented = ArrayPool<byte>.Shared.Rent((int)stream.Length);
                var buffer = new MemoryStream(rented, 0, (int)stream.Length, true);
                stream.CopyTo(buffer);
                stream.Dispose();
                stream = buffer;
            }

            var rqb = MessagePackSerializer.Deserialize<RequestCodeBuffer>(stream, Options);
            var type = table.GetTypeFor(rqb.RequestCode);
            stream.Seek(0, SeekOrigin.Begin);
            var obj = MessagePackSerializer.Deserialize(type, stream, Options) as RESTRequestBase<TRESTCode>
                ?? throw new InvalidDataException($"Could not obtain a {typeof(RESTRequestBase<TRESTCode>)} from stream");
            obj.Deserialized(this);
            return obj;
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public async Task<RESTRequestBase<TRESTCode>> DeserializeAsync(Stream stream, RequestTypeTable<TRESTCode> table)
    {
        if (stream.Length > int.MaxValue)
            throw new ArgumentException("The stream is too large", nameof(stream));

        byte[]? rented = null;
        try
        {
            if (stream.CanSeek is false)
            {
                rented = ArrayPool<byte>.Shared.Rent((int)stream.Length);
                var buffer = new MemoryStream(rented, 0, (int)stream.Length, true);
                await stream.CopyToAsync(buffer);
                await stream.DisposeAsync();
                stream = buffer;
            }

            var rqb = await MessagePackSerializer.DeserializeAsync<RequestCodeBuffer>(stream, Options);
            var type = table.GetTypeFor(rqb.RequestCode);
            stream.Seek(0, SeekOrigin.Begin);
            var obj = await MessagePackSerializer.DeserializeAsync(type, stream, Options) as RESTRequestBase<TRESTCode>
                ?? throw new InvalidDataException($"Could not obtain a {typeof(RESTRequestBase<TRESTCode>)} from stream");
            obj.Deserialized(this);
            return obj;
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
