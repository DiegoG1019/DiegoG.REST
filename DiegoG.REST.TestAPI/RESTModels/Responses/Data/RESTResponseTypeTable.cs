namespace DiegoG.REST.TestAPI.RESTModels.Responses.Data;

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
