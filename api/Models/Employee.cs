using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Index(nameof(Name), nameof(Email), nameof(Phone), IsUnique = true)]
public class Employee : Entity
{
    [EmailAddress]
    public required string Email { get; set; }
    [Phone]
    [RegularExpression("^(8|9)[0-9]{7}$")]
    [MaxLength(8)]
    public required string Phone { get; set; }
    public int Gender { get; set; }
    public string? CompanyId { get; set; }
    public Company? Company { get; set; }
    public DateTime? StartDate { get; set; }

    [NotMapped]
    public int DaysWorked => StartDate == null ? 0 : DateTime.UtcNow.Subtract((DateTime)StartDate!).Days;
}