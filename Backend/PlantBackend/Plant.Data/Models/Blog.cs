using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class Blog : BaseEntity
{
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public int? UserId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}
