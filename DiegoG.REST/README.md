# DiegoG.REST

[![NuGet Package](https://img.shields.io/badge/NuGet-v1.0.1-blue)](https://www.nuget.org/packages/DiegoG.REST.Core)

**Provides base types and utilities useful for creating standardized models for a REST API and consumers of such an API**

## Types
### RESTObject
A base type for model objects that provides methods to react when serialization and deserialization (when using a [IRESTObjectSerializer](#IRESTObjectSerializer) or other compliant serializer) and a members that provide useful metadata about the object itself, allowing it to be identified as the actually appropriate model when such information is otherwise lacking, for example, when receiving a request from an API that could send a variety of responses.

### IRESTObjectSerializer
An interface that defines methods and members that permit easy deserialization of the appropriate type for any given model using its `RESTObject<TObjectCode>.Code`
- Note that this library does *not* provide any default serializers, see [DiegoG.REST.Json](https://www.nuget.org/packages/DiegoG.REST.Json)

### RESTObjectTypeTable
An abstract class that, at its root, is a dictionary that returns type information for a given REST model created using this library, discriminated by its `RESTObject<TObjectCode>.Code`

## Examples

#### Base Response type
```C#
public enum ResponseCodeEnum : uint
{
    TestA = 0,

    ErrorResponse = 100
}


public readonly record struct ResponseCode(ResponseCodeEnum Code) : IEquatable<ResponseCode>
{
    public string Name { get; } = Code.ToString();

    public static implicit operator ResponseCodeEnum(ResponseCode code) => code.Code;
    public static implicit operator ResponseCode(ResponseCodeEnum code) => new(code);
}

public abstract class ResponseBase : RESTObject<ResponseCode>
{
    protected ResponseBase(ResponseCode code) : base(code)
    {
    }
}
```

#### Response Type Table
```C#
public class RESTResponseTypeTable : RESTObjectTypeTable<ResponseCode>
{
    private RESTResponseTypeTable() { }

    public static RESTResponseTypeTable Instance { get; } = new();

    public override Type GetTypeFor(ResponseCode code)
        => code.Code switch
        {
            ResponseCodeEnum.TestA => typeof(TestResponseA),

            ResponseCodeEnum.ErrorResponse => typeof(ErrorResponse),
            _ => throw new ArgumentException($"Unknown ResponseCode {code.Code}"),
        };

    protected override void AddType(Type type, ResponseCode code)
    {
        throw new NotImplementedException("Adding types to this table is not supported");
    }
}
```

#### Error Response
```C#
public class ErrorResponse : ResponseBase
{
    public ErrorResponse(params Exception?[] exceptions) : base(new ResponseCode(ResponseCodeEnum.ErrorResponse))
    {
        Errors = exceptions.Where(x => x is not null).Select(x => x!.Message).ToImmutableArray();
    }

    public ErrorResponse(params string[] errors) : base(new ResponseCode(ResponseCodeEnum.ErrorResponse))
    {
        Errors = ImmutableArray.Create(errors);
    }

    public ErrorResponse(IEnumerable<string>? errors = null, IEnumerable<Exception>? exceptions = null) : base(new ResponseCode(ResponseCodeEnum.ErrorResponse))
    {
        IEnumerable<string> query = errors ?? Array.Empty<string>();
        query = exceptions is null ? query : query.Concat(exceptions.Where(x => x is not null).Select(x => x.Message));
        Errors = query.ToImmutableArray();
    }

    public ImmutableArray<string> Errors { get; init; }
}
```

#### TestResponseA
```C#
public class TestResponseA : ResponseBase
{
    public string LeResponse { get; init; }

    public TestResponseA(string leResponse) : base(ResponseCodeEnum.TestA)
    {
        LeResponse = leResponse ?? throw new ArgumentNullException(nameof(leResponse));
    }
}
```

#### Example Usage
```C#
public static async Task Main(string[] args)
{
    var serializer = new JsonRESTSerializer<ResponseCode>();
    RESTObject<ResponseCode> resp = args.Contains("do-error") ? new ErrorResponse("My bad", "Whoops") : new TestResponseA("Superb!");

    var ms = new MemoryStream(); // usually, this wouldn't be a MemoryStream

    serializer.Serialize(resp, ms);

    // ---------------

    var r = serializer.Deserialize(ms, RESTResponseTypeTable.Instance);
    // This will deserialize the appropriate response
}
```