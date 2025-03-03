using System.ComponentModel.DataAnnotations;
using System;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? UserFname { get; set; }
    public string? UserLname { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    [Required]
    [EmailAddress]
    public string? UserEmail { get; set; }
    public string UserRole { get; set; } = "Customer";  // Default role
    public DateTime DateCreated { get; set; } = DateTime.Now;  // Set default creation date
}

