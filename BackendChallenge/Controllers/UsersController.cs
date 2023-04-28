using BackendChallenge.Data;
using BackendChallenge.Models;
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
    /// <summary>
    /// Returns all active users for the company that the querying user belongs to 
    /// Response Type: array of UserResponses - userId, firstName, lastName
    /// </summary>
    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> Index([FromHeader]String userToken, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(userToken))
            {
                return Unauthorized();
            }
        
        // get token entry from db
        var curUserToken = await _db.UserTokens.FindAsync(userToken);
        if (curUserToken == null) {
            return NotFound();
        }
        // use current user entry by userId
        var curUser = await _db.Users.FindAsync(curUserToken.UserId);
        if (curUser == null) {
            return NotFound();
        }
        var users = await _db.Users
                .Where(u => u.CompanyId == curUser.CompanyId)
                .Select(u => new UserResponse { UserId = u.UserId, FirstName = u.FirstName, LastName = u.LastName })
                .ToListAsync(token);

        return Ok(users);
    }
}