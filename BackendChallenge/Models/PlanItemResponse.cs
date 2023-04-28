using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendChallenge.Enums;

namespace BackendChallenge.Models;

/// <summary>
/// Represents a Learning Plan item DTO
/// </summary>
public class PlanItemResponse
{
    // Unique ID for this LearningPlanItem
    public int LearningPlanItemId { get; set; }

    // The type of LearningPlanItme that this represents 
    public LearningItemType LearningItemType { get; set; }
    
    public string? LearningItemName { get; set; }

    public int ItemId { get; set; }
}