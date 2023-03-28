using DiegoG.REST.TestAPI.RESTModels.Responses.Data;

namespace DiegoG.REST.TestAPI.RESTModels.Responses;

public class TestResponseA : ResponseBase
{
    public string LeResponse { get; init; }

    public TestResponseA(string leResponse) : base(ResponseCodeEnum.TestA)
    {
        LeResponse = leResponse ?? throw new ArgumentNullException(nameof(leResponse));
    }
}
