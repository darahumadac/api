using System.ComponentModel.DataAnnotations;

namespace api.Models;


public class Entity
{
    public string Id { get; set; } = null!;

    [MaxLength(10)]
    public required string Name { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public byte[] ETag { get; set; } = default!;

}