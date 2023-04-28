using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendChallenge.Enums;

namespace BackendChallenge.Models;

/// <summary>
/// Represents a learning plan DTO
/// </summary>
public class LearningPlanResponse
{
    public LearningPlanResponse()
    {
       PlanItems = new Collection<PlanItemResponse>();
    }

    // The ID of the user whose user plan this is
    public int UserId { get; set; }
    
    public virtual ICollection<PlanItemResponse> PlanItems { get; set; }
}