using System.ComponentModel.DataAnnotations;

namespace VisaAppointmentSystem.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

