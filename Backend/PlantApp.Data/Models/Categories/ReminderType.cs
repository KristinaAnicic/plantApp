using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class ReminderType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Reminder>? Reminders { get; set; }
}
