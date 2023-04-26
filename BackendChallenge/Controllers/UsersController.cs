using BackendChallenge.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly AppDbContext _db;

    public UsersController(ILogger<UsersController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<string>> Index(CancellationToken token)
    {
        var x = await _db.Users.FirstOrDefaultAsync(token);
        return Ok("it's working");
    }

    [HttpGet(Name = "GetMyStaff")]
    public async Task<ActionResult<string>> Index2(CancellationToken token)
    {
        var x = await _db.Users.FirstOrDefaultAsync(token);
        return Ok("my staff, it's working");
    }
}