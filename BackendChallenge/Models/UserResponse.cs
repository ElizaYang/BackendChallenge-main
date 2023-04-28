namespace BackendChallenge.Models;

/// <summary>
/// Represents a user DTO in the system
/// </summary>
public class UserResponse
{
    // Unique ID for the user
    public int UserId { get; set; }
    
    // First name of the user
    public string? FirstName { get; set; }
    
    // Last name of the user
    public string? LastName { get; set; }
}