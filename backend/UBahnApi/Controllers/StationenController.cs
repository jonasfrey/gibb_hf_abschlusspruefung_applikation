using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBahnApi.Data;
using UBahnApi.DTOs;
using UBahnApi.Models;

namespace UBahnApi.Controllers;

[ApiController]
[Route("api/stationen")]
public class StationenController(UBahnContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<StationDto>> GetAll()
    {
        return await db.Stationen
            .Include(s => s.Linie)
            .OrderBy(s => s.LinieId)
            .ThenBy(s => s.Position)
            .Select(s => new StationDto(s.Id, s.Name, s.LinieId, s.Linie.Name, s.Position))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StationDto>> GetById(int id)
    {
        var s = await db.Stationen.Include(s => s.Linie).FirstOrDefaultAsync(s => s.Id == id);
        if (s is null) return NotFound();
        return new StationDto(s.Id, s.Name, s.LinieId, s.Linie.Name, s.Position);
    }

    [HttpPost]
    public async Task<ActionResult<StationDto>> Create(StationCreateDto dto)
    {
        var linie = await db.Linien.FindAsync(dto.LinieId);
        if (linie is null) return BadRequest("Linie nicht gefunden.");

        var station = new Station
        {
            Name = dto.Name,
            LinieId = dto.LinieId,
            Position = dto.Position
        };
        db.Stationen.Add(station);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = station.Id },
            new StationDto(station.Id, station.Name, station.LinieId, linie.Name, station.Position));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StationUpdateDto dto)
    {
        var station = await db.Stationen.FindAsync(id);
        if (station is null) return NotFound();

        var linie = await db.Linien.FindAsync(dto.LinieId);
        if (linie is null) return BadRequest("Linie nicht gefunden.");

        station.Name = dto.Name;
        station.LinieId = dto.LinieId;
        station.Position = dto.Position;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var station = await db.Stationen.FindAsync(id);
        if (station is null) return NotFound();

        var fahrzeiten = db.Fahrzeiten.Where(f => f.VonStationId == id || f.NachStationId == id);
        db.Fahrzeiten.RemoveRange(fahrzeiten);
        db.Stationen.Remove(station);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
