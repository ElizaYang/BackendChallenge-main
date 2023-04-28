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
    public async Task<ActionResult<IEnumerable<UserResponses>>> Index([FromHeader]String userToken, CancellationToken token)
    {
        if (userToken == null) {
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

        return await _db.Users
            .Where(x => x.CompanyId == curUser.CompanyId)
            .Select(user => UserToDTO(user))
            .ToListAsync();
    }

    private static UserResponses UserToDTO(User user) => 
        new UserResponses
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

    
}