using BackendChallenge.Data;
using BackendChallenge.Enums;
using BackendChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("[controller]")]
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
    /// Returns the incentives that a user is able to see and is eligible to apply for. 
    /// Response Type: IncentiveResponse
    /// </summary>
    [HttpGet(Name = "GetIncentives")]
    public async Task<ActionResult<IncentiveResponse>> Index([FromHeader] String userToken, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(userToken))
        {
            return Unauthorized();
        }

        // get token entry from db
        var curUserToken = await _db.UserTokens.FindAsync(userToken);
        if (curUserToken == null)
        {
            return NotFound();
        }
        // use current user entry by userId
        var curUser = await _db.Users.FindAsync(curUserToken.UserId);
        if (curUser == null)
        {
            return NotFound();
        }
        var curCompanyId = curUser.CompanyId;
        var tenureDays = curUser.TenureDays;

        // get current user's role
        RoleEligibility curRole;
        var isManager = await _db.ManagementRelationships.FirstOrDefaultAsync(r => r.ManagerId == curUser.UserId);
        if (isManager == null) {
            curRole = RoleEligibility.IndividualContributor;
        } else {
            curRole = RoleEligibility.Manager;
        }

        // Get the IDs of the incentives that the user is eligible for based on their tenure and role.
        var eligibleIncentiveIds = await _db.Incentives
            .Where(i => i.CompanyId == curCompanyId &&
                        i.ServiceRequirementDays <= tenureDays &&
                        (i.RoleEligibility == 0 || 
                        i.RoleEligibility == curRole))
            .Select(i => i.IncentiveId)
            .ToListAsync(token);

        // Get the details of the eligible incentives.
        var eligibleIncentives = await _db.Incentives
            .Where(i => eligibleIncentiveIds.Contains(i.IncentiveId))
            .Select(i => new EligibleIncentiveResponse
            {
                IncentiveId = i.IncentiveId,
                IncentiveName = i.IncentiveName,
                ServiceRequirement = i.ServiceRequirementDays,
                RoleEligibility = i.RoleEligibility
            })
            .ToListAsync(token);

        return new IncentiveResponse
        {
            UserId = curUser.UserId,
            EligibleIncentiveResponse = eligibleIncentives
        };
    }
}
