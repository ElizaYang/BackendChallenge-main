using BackendChallenge.Data;
using BackendChallenge.Enums;
using BackendChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("incentives")]
public class IncentivesController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly AppDbContext _db;

    public IncentivesController(ILogger<UsersController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    /// <summary>
    /// Returns the incentives that a user is able to see and is eligible to apply for
    /// </summary>
    /// <param name="userToken">The user token passed in the request header</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>IncentiveResponse including userId, incentives: array of EligibleIncentiveResponse</returns>
    [HttpGet]
    public async Task<ActionResult<IncentiveResponse>> GetIncentives([FromHeader] String userToken, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(userToken))
        {
            return Unauthorized();
        }

        // Get the current user based on the given user token
        var curUser = await GetCurrentUser(userToken, token);
        if (curUser == null)
        {
            return NotFound();
        }

        // Returns a list of eligible incentives for the given user
        var eligibleIncentives = await GetEligibleIncentives(curUser, token);

        return new IncentiveResponse
        {
            UserId = curUser.UserId,
            EligibleIncentiveResponse = eligibleIncentives
        };
    }
    private async Task<User?> GetCurrentUser(string userToken, CancellationToken token)
    {
        var curUserToken = await _db.UserTokens.FindAsync(userToken);
        if (curUserToken == null)
        {
            return null;
        }

        return await _db.Users.FindAsync(curUserToken.UserId);
    }

    /// <summary>
    /// Retrieves a list of eligible incentives for the specified user.
    /// An incentive is considered eligible if it meets the following criteria:
    /// - Belongs to the same company as the user.
    /// - Has a service requirement days less than or equal to the user's tenure days.
    /// - Has a role eligibility of all or matches the user's current role.
    /// </summary>
    private async Task<List<EligibleIncentiveResponse>> GetEligibleIncentives(User user, CancellationToken token)
    {
        var tenureDays = user.TenureDays;
        var curCompanyId = user.CompanyId;
        var curRole = await GetCurrentUserRole(user, token);

        var eligibleIncentiveIds = await _db.Incentives
            .Where(i => i.CompanyId == curCompanyId &&
                        i.ServiceRequirementDays <= tenureDays &&
                        (i.RoleEligibility == 0 ||
                         i.RoleEligibility == curRole))
            .Select(i => i.IncentiveId)
            .ToListAsync(token);

        return await _db.Incentives
            .Where(i => eligibleIncentiveIds.Contains(i.IncentiveId))
            .Select(i => new EligibleIncentiveResponse
            {
                IncentiveId = i.IncentiveId,
                IncentiveName = i.IncentiveName,
                ServiceRequirement = i.ServiceRequirementDays,
                RoleEligibility = i.RoleEligibility
            })
            .ToListAsync(token);
    }

    private async Task<RoleEligibility> GetCurrentUserRole(User user, CancellationToken token)
    {
        var isManager = await _db.ManagementRelationships.FirstOrDefaultAsync(r => r.ManagerId == user.UserId, token);
        return isManager == null ? RoleEligibility.IndividualContributor : RoleEligibility.Manager;
    }
}

