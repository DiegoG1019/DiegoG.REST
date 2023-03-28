namespace DiegoG.REST.TestAPI.RESTModels.Responses.Data;

public abstract class ResponseBase : RESTObject<ResponseCode>
{
    protected ResponseBase(ResponseCode code) : base(code)
    {
    }
}
