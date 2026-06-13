using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBahnApi.Data;

namespace UBahnApi.Controllers;

public record FahrtResultDto(
    string StartStation,
    string ZielStation,
    int AnzahlZwischenstationen,
    List<string> Zwischenstationen,
    int GesamtdauerMinuten
);

[ApiController]
[Route("api/fahrt")]
public class FahrtController(UBahnContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<FahrtResultDto>> GetFahrt([FromQuery] int start, [FromQuery] int ziel)
    {
        if (start == ziel)
            return BadRequest("Start- und Zielstation sind identisch.");

        var startStation = await db.Stationen.FindAsync(start);
        var zielStation = await db.Stationen.FindAsync(ziel);

        if (startStation is null) return NotFound($"Startstation mit ID {start} nicht gefunden.");
        if (zielStation is null) return NotFound($"Zielstation mit ID {ziel} nicht gefunden.");

        if (startStation.LinieId != zielStation.LinieId)
            return BadRequest($"Die Stationen liegen auf unterschiedlichen Linien. Umsteigen wird nicht unterstützt.");

        var alleStationen = await db.Stationen
            .Where(s => s.LinieId == startStation.LinieId)
            .OrderBy(s => s.Position)
            .ToListAsync();

        var alleFahrzeiten = await db.Fahrzeiten
            .Include(f => f.VonStation)
            .Include(f => f.NachStation)
            .Where(f => f.VonStation.LinieId == startStation.LinieId)
            .ToListAsync();

        int posStart = startStation.Position;
        int posZiel = zielStation.Position;

        int von = Math.Min(posStart, posZiel);
        int bis = Math.Max(posStart, posZiel);

        var strecke = alleStationen.Where(s => s.Position >= von && s.Position <= bis).ToList();

        int gesamtdauer = 0;
        for (int i = 0; i < strecke.Count - 1; i++)
        {
            var a = strecke[i];
            var b = strecke[i + 1];
            var fz = alleFahrzeiten.FirstOrDefault(f =>
                (f.VonStationId == a.Id && f.NachStationId == b.Id) ||
                (f.VonStationId == b.Id && f.NachStationId == a.Id));
            if (fz is not null) gesamtdauer += fz.DauerMinuten;
        }

        var zwischenStationen = strecke
            .Where(s => s.Id != start && s.Id != ziel)
            .Select(s => s.Name)
            .ToList();

        return Ok(new FahrtResultDto(
            startStation.Name,
            zielStation.Name,
            zwischenStationen.Count,
            zwischenStationen,
            gesamtdauer
        ));
    }
}
