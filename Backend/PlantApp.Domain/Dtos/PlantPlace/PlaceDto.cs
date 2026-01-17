namespace PlantApp.Domain.Dtos.PlantPlace;

public class PlaceDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public int NumOfPlants { get; set; }
    public string? Note { get; set; }   
}
