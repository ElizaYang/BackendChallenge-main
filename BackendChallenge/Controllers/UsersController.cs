using BackendChallenge.Data;
using BackendChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("users")]
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
    /// </summary>
    /// <param name="userToken">The user token passed in the request header</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>An array of UserResponses including userId, firstName, lastName</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers([FromHeader] String userToken, CancellationToken token)
    {
        // Check if user token is empty or null
        if (string.IsNullOrWhiteSpace(userToken))
        {
            return Unauthorized();
        }
        // Get current user by userToken
        var currentUser = await GetCurrentUser(userToken);
        if (currentUser == null)
        {
            return NotFound();
        }
        // Get users for the company of the current user
        var users = await GetUsersForCompany(currentUser.CompanyId, token);

        return Ok(users);
    }
    private async Task<User?> GetCurrentUser(string userToken)
    {
        var userTokenEntity = await _db.UserTokens.FindAsync(userToken);
        if (userTokenEntity == null)
        {
            return null;
        }

        var user = await _db.Users.FindAsync(userTokenEntity.UserId);
        return user;
    }

    private async Task<IEnumerable<UserResponse>> GetUsersForCompany(int companyId, CancellationToken token)
    {
        var users = await _db.Users
            .Where(u => u.CompanyId == companyId)
            .Select(u => new UserResponse { UserId = u.UserId, FirstName = u.FirstName, LastName = u.LastName })
            .ToListAsync(token);

        return users;
    }
}
