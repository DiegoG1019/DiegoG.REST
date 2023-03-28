using DiegoG.REST.ASPNET;
using DiegoG.REST.TestAPI.RESTModels.Responses;
using DiegoG.REST.TestAPI.RESTModels.Responses.Data;
using Microsoft.AspNetCore.Mvc;

namespace DiegoG.REST.TestAPI.Controllers;

public readonly record struct TestRequestBody(string Hello);

[AddModelStateFeature]
[ApiController]
[Route("api/test")]
public class TestController : Controller
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok(new TestResponseA("Test Succesful"));
    }

    [HttpPost]
    public IActionResult TestBody([FromBody] TestRequestBody arg)
    {
        return Ok(new TestResponseA("Test Succesful"));
    }

    [HttpGet("error")]
    public IActionResult TestError()
    {
        throw new InvalidOperationException("Test Exception");
    }
}
