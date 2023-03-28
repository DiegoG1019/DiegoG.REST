using System.Collections.Concurrent;
using DiegoG.RESTBase.Requests;
using MessagePack;
using MessagePack.Formatters;

namespace DiegoG.REST.MessagePack;

public class RESTFormatterResolver<TRESTCode>
    : IFormatterResolver
    where TRESTCode : struct, Enum, IEquatable<TRESTCode>
{
    private readonly ConcurrentDictionary<Type, IMessagePackFormatter> ConstructedTypesDict = new();

    public virtual IMessagePackFormatter<T>? GetFormatter<T>()
        => typeof(T).IsAssignableTo(typeof(RESTRequestBase<TRESTCode>)) is false
            ? null
            : (IMessagePackFormatter<T>)ConstructedTypesDict.GetOrAdd(typeof(T), Add);

    private IMessagePackFormatter Add(Type type)
        => (IMessagePackFormatter)Activator.CreateInstance(typeof(RequestMessagePackFormatter<,>).MakeGenericType(type, typeof(TRESTCode)))!;
}
