using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendChallenge.Enums;

namespace BackendChallenge.Models;

/// <summary>
/// Represents an incentiveResponse DTO
/// </summary>
public class IncentiveResponse
{
    public IncentiveResponse()
    {
       EligibleIncentiveResponse = new Collection<EligibleIncentiveResponse>();
    }

    // The ID of the user whose incentive this is
    public int UserId { get; set; }
    
    public virtual ICollection<EligibleIncentiveResponse> EligibleIncentiveResponse { get; set; }
}