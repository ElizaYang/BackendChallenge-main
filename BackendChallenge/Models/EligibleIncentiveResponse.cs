using BackendChallenge.Enums;

namespace BackendChallenge.Models;

/// <summary>
/// Represents an eligibleIncentiveResponse DTO
/// </summary>
public class EligibleIncentiveResponse
{
    // Unique Id for the incentive
    public int IncentiveId { get; set; }
    
    // Display name of the incentive
    public string? IncentiveName { get; set; }
    
    // How many days an employee needs to work at the company to be eligible
    public int ServiceRequirement { get; set; }
    
    // What roles are eligible for this incentive
    public RoleEligibility RoleEligibility { get; set; }
}