using System.Runtime.CompilerServices;
using DiegoG.RESTBase.Requests;
using MessagePack;
using MessagePack.Formatters;

namespace DiegoG.REST.MessagePack;

public class RequestMessagePackFormatter<TRequest, TRequestCode> : IMessagePackFormatter<TRequest>
    where TRequest : RESTRequestBase<TRequestCode>
    where TRequestCode : struct, Enum, IEquatable<TRequestCode>
{
    public unsafe void Serialize(ref MessagePackWriter writer, TRequest value, MessagePackSerializerOptions options)
    {
        var code = value.RequestCode;
        {
            ulong buffer = 0;
            Span<byte> bytes = new(&buffer, sizeof(ulong));
            Span<byte> codebytes = new(&code, Unsafe.SizeOf<TRequestCode>());

            for (int i = 0; i < codebytes.Length; i++)
                bytes[i] = codebytes[i];
            // the rest are already 0

            writer.WriteUInt64(buffer);
        }
    }

    public unsafe TRequest Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        TRequestCode code = default;
        {
            ulong buffer = reader.ReadUInt64();
            Span<byte> bytes = new(&buffer, sizeof(ulong));
            Span<byte> codebytes = new(&code, Unsafe.SizeOf<TRequestCode>());

            for (int i = 0; i < codebytes.Length; i++)
                codebytes[i] = bytes[i];
        }
    }
}
