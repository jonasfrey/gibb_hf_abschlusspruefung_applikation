using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBahnApi.Data;
using UBahnApi.DTOs;
using UBahnApi.Models;

namespace UBahnApi.Controllers;

[ApiController]
[Route("api/fahrzeiten")]
public class FahrzeitenController(UBahnContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<FahrzeitDto>> GetAll()
    {
        return await db.Fahrzeiten
            .Include(f => f.VonStation)
            .Include(f => f.NachStation)
            .Select(f => new FahrzeitDto(
                f.Id,
                f.VonStationId, f.VonStation.Name,
                f.NachStationId, f.NachStation.Name,
                f.DauerMinuten))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FahrzeitDto>> GetById(int id)
    {
        var f = await db.Fahrzeiten
            .Include(f => f.VonStation)
            .Include(f => f.NachStation)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (f is null) return NotFound();
        return new FahrzeitDto(f.Id, f.VonStationId, f.VonStation.Name, f.NachStationId, f.NachStation.Name, f.DauerMinuten);
    }

    [HttpPost]
    public async Task<ActionResult<FahrzeitDto>> Create(FahrzeitCreateDto dto)
    {
        var von = await db.Stationen.Include(s => s.Linie).FirstOrDefaultAsync(s => s.Id == dto.VonStationId);
        var nach = await db.Stationen.Include(s => s.Linie).FirstOrDefaultAsync(s => s.Id == dto.NachStationId);

        if (von is null || nach is null) return BadRequest("Station nicht gefunden.");
        if (von.LinieId != nach.LinieId) return BadRequest("Stationen müssen auf der gleichen Linie liegen.");
        if (Math.Abs(von.Position - nach.Position) != 1) return BadRequest("Fahrzeiten können nur zwischen direkt aufeinanderfolgenden Stationen erfasst werden.");

        var fahrzeit = new Fahrzeit
        {
            VonStationId = dto.VonStationId,
            NachStationId = dto.NachStationId,
            DauerMinuten = dto.DauerMinuten
        };
        db.Fahrzeiten.Add(fahrzeit);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = fahrzeit.Id },
            new FahrzeitDto(fahrzeit.Id, von.Id, von.Name, nach.Id, nach.Name, fahrzeit.DauerMinuten));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FahrzeitUpdateDto dto)
    {
        var fahrzeit = await db.Fahrzeiten.FindAsync(id);
        if (fahrzeit is null) return NotFound();

        var von = await db.Stationen.FindAsync(dto.VonStationId);
        var nach = await db.Stationen.FindAsync(dto.NachStationId);
        if (von is null || nach is null) return BadRequest("Station nicht gefunden.");

        fahrzeit.VonStationId = dto.VonStationId;
        fahrzeit.NachStationId = dto.NachStationId;
        fahrzeit.DauerMinuten = dto.DauerMinuten;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fahrzeit = await db.Fahrzeiten.FindAsync(id);
        if (fahrzeit is null) return NotFound();
        db.Fahrzeiten.Remove(fahrzeit);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
