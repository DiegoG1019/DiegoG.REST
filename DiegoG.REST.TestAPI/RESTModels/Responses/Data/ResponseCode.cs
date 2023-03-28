namespace DiegoG.REST.TestAPI.RESTModels.Responses.Data;

public readonly record struct ResponseCode(ResponseCodeEnum Code) : IEquatable<ResponseCode>
{
    public string Name { get; } = Code.ToString();

    public static implicit operator ResponseCodeEnum(ResponseCode code) => code.Code;
    public static implicit operator ResponseCode(ResponseCodeEnum code) => new(code);
}
