namespace PlantApp.Domain.Dtos
{
    public class ImageDto
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string? Copyright { get; set; }
    }
}
