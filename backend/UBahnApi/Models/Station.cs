namespace UBahnApi.Models;

public class Station
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LinieId { get; set; }
    public int Position { get; set; }
    public Linie Linie { get; set; } = null!;
}
