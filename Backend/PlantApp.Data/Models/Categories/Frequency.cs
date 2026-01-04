using PlantApp.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models.Categories;

public class Frequency : IReferenceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Reminder>? Reminders { get; set; }
    public ICollection<ReminderHistory>? ReminderHistory { get; set; }
}
