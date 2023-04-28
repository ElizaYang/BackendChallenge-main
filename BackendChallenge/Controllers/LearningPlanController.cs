using BackendChallenge.Data;
using BackendChallenge.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("[controller]")]
public class LearningPlanController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly AppDbContext _db;

    public LearningPlanController(ILogger<UsersController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    /// <summary>
    /// Returns all active users for the company that the querying user belongs to 
    /// Response Type: array of UserResponses - userId, firstName, lastName
    /// </summary>
    [HttpGet(Name = "GetLearningPlan")]
    public async Task <ActionResult<LearningPlanResponse>> Index([FromHeader] String userToken, CancellationToken token)
    {
        if (userToken == null) {
            return Unauthorized();
        }
        
        // get token entry from db
        var curUserToken = await _db.UserTokens.FindAsync(userToken);
        if (curUserToken == null) {
            return NotFound();
        }
        // get current userId 
        var curId = curUserToken.UserId;
        var query = from p in _db.LearningPlans
                    join i in _db.LearningPlanItems on p.LearningPlanId equals i.LearningPlanId
                    join c in _db.Courses on i.CourseId equals c.CourseId into cJoin // left outer join with courseTable
                    from cItem in cJoin.DefaultIfEmpty()
                    join inct in _db.Incentives on i.IncentiveId equals inct.IncentiveId into inctJoin // left outer join with incentiveTable
                    from inctItem in inctJoin.DefaultIfEmpty()
                    where p.UserId == curId
                    select new { p.LearningPlanId, p.UserId, 
                                i.LearningPlanItemId, i.LearningItemType,
                                LearningItemName = cItem.CourseName ?? inctItem.IncentiveName, // choose name based on which table has a value
                                ItemId = i.IncentiveId != null ? inctItem.IncentiveId : cItem.CourseId // choose id based on learning item type
            
                    };

        var groupedQuery = query.GroupBy(x => x.LearningPlanId);

        var learningPlanResponses = groupedQuery.Select(group => new LearningPlanResponse
        {
            UserId = group.First().UserId,
            PlanItems = group.Select(item => new PlanItemResponse
            {
                LearningPlanItemId = item.LearningPlanItemId,
                LearningItemType = item.LearningItemType,
                LearningItemName = item.LearningItemName,
                ItemId = item.ItemId
            }).ToList()
        }).ToList();


        return Ok(learningPlanResponses);
    }
}