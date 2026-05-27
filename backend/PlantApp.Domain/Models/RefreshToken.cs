using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

[Index(nameof(Token), IsUnique = true)]
[Index(nameof(UserId))]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiryTime { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public bool IsActive => RevokedAt == null && ExpiryTime > DateTime.UtcNow;
}
