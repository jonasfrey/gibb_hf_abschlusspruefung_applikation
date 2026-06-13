namespace UBahnApi.Models;

public class Linie
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Station> Stationen { get; set; } = [];
}
