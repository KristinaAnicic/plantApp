namespace PlantApp.Domain.Dtos;

public class ListResponse<T>
{
    public int Total {  get; set; }
    public List<T>? Items { get; set; } 
}
