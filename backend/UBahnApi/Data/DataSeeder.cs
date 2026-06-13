using UBahnApi.Models;

namespace UBahnApi.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(UBahnContext db)
    {
        if (db.Linien.Any()) return;

        var hauptlinie = new Linie { Name = "Hauptlinie" };
        var stadtlinie = new Linie { Name = "Stadtlinie" };
        db.Linien.AddRange(hauptlinie, stadtlinie);
        await db.SaveChangesAsync();

        var parkhaus        = new Station { Name = "Parkhaus",        Linie = hauptlinie, Position = 1 };
        var einkauf         = new Station { Name = "Einkaufsstrasse", Linie = hauptlinie, Position = 2 };
        var hbfHaupt        = new Station { Name = "Hauptbahnhof",    Linie = hauptlinie, Position = 3 };
        var flughafen       = new Station { Name = "Flughafen",       Linie = hauptlinie, Position = 4 };
        var oberstadt       = new Station { Name = "Oberstadt",       Linie = stadtlinie, Position = 1 };
        var hbfStadt        = new Station { Name = "Hauptbahnhof",    Linie = stadtlinie, Position = 2 };
        var museum          = new Station { Name = "Museum",          Linie = stadtlinie, Position = 3 };
        var unterstadt      = new Station { Name = "Unterstadt",      Linie = stadtlinie, Position = 4 };
        db.Stationen.AddRange(parkhaus, einkauf, hbfHaupt, flughafen, oberstadt, hbfStadt, museum, unterstadt);
        await db.SaveChangesAsync();

        db.Fahrzeiten.AddRange(
            new Fahrzeit { VonStation = parkhaus,  NachStation = einkauf,   DauerMinuten = 4 },
            new Fahrzeit { VonStation = einkauf,   NachStation = hbfHaupt,  DauerMinuten = 7 },
            new Fahrzeit { VonStation = hbfHaupt,  NachStation = flughafen, DauerMinuten = 9 },
            new Fahrzeit { VonStation = oberstadt, NachStation = hbfStadt,  DauerMinuten = 5 },
            new Fahrzeit { VonStation = hbfStadt,  NachStation = museum,    DauerMinuten = 2 },
            new Fahrzeit { VonStation = museum,    NachStation = unterstadt, DauerMinuten = 3 }
        );
        await db.SaveChangesAsync();
    }
}
