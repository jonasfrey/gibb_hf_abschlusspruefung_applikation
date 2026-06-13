namespace UBahnApi.Models;

public class Fahrzeit
{
    public int Id { get; set; }
    public int VonStationId { get; set; }
    public int NachStationId { get; set; }
    public int DauerMinuten { get; set; }
    public Station VonStation { get; set; } = null!;
    public Station NachStation { get; set; } = null!;
}
