using BackendChallenge.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly AppDbContext _db;

    public TestController(ILogger<TestController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    /// <summary>
    /// This is a testing controller to test API connection.
    /// </summary>
    public async Task<ActionResult<string>> Test(CancellationToken token)
    {
        return Ok("it's working");
    }
}