using BackendChallenge.Data;
using BackendChallenge.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendChallenge.Controllers;

[ApiController]
[Route("learning-plan")]
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
    /// Returns the learning plan for the querying user
    /// </summary>
    /// <param name="userToken">The user token passed in the request header</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>LearningPlanResponse including userId, planItems: array of PlanItemResponse</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LearningPlanResponse>>> GetPlans([FromHeader] string userToken, CancellationToken token)
    {
        // Check if user token is empty or null
        if (string.IsNullOrWhiteSpace(userToken))
        {
            return Unauthorized();
        }

        // Get token entry from database
        var curUserToken = await _db.UserTokens.FindAsync(userToken);
        if (curUserToken == null)
        {
            return NotFound();
        }

        // Retrieve current user ID
        var curId = curUserToken.UserId;

        // Query learning plans and related items, courses and incentives
        var query = from p in _db.LearningPlans
                    join i in _db.LearningPlanItems on p.LearningPlanId equals i.LearningPlanId
                    join c in _db.Courses on i.CourseId equals c.CourseId into cJoin // left outer join with courseTable
                    from cItem in cJoin.DefaultIfEmpty()
                    join inct in _db.Incentives on i.IncentiveId equals inct.IncentiveId into inctJoin // left outer join with incentiveTable
                    from inctItem in inctJoin.DefaultIfEmpty()
                    where p.UserId == curId
                    select new
                    {
                        p.LearningPlanId,
                        p.UserId,
                        i.LearningPlanItemId,
                        i.LearningItemType,
                        LearningItemName = cItem.CourseName ?? inctItem.IncentiveName, // choose name based on which table has a value
                        ItemId = i.IncentiveId != null ? inctItem.IncentiveId : cItem.CourseId // choose id based on learning item type

                    };

        // Group query results by learning plan ID
        var groupedQuery = query.GroupBy(x => x.LearningPlanId);

        // Convert grouped query results to learning plan responses
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