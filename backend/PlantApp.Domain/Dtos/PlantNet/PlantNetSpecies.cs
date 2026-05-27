namespace PlantApp.Domain.Dtos.PlantNet;

public class PlantNetSpecies
{
    public string? ScientificNameWithoutAuthor { get; set; }
    public string? ScientificNameAuthorship { get; set; }
    public PlantNetScientificName? Genus { get; set; }
    public PlantNetScientificName? Family { get; set; }
    public List<string>? CommonNames {  get; set; }
    public string? ScientificName { get; set; }
}
