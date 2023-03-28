using System.Collections.Immutable;
using DiegoG.REST.ASPNET;
using DiegoG.REST.TestAPI.RESTModels.Responses.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiegoG.REST.TestAPI.RESTModels.Responses;

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
