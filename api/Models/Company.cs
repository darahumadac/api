using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

[Index(nameof(Name), nameof(Location), IsUnique = true)]
public class Company : Entity
{
    [MaxLength(256)]
    public required string Description { get; set; }
    [MaxLength(100)]
    public required string Location { get; set; }

    public List<Employee> Employees { get; set; } = null!;
}