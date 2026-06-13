using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBahnApi.Data;
using UBahnApi.DTOs;
using UBahnApi.Models;

namespace UBahnApi.Controllers;

[ApiController]
[Route("api/linien")]
public class LinienController(UBahnContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<LinieDetailDto>> GetAll()
    {
        var linien = await db.Linien
            .Include(l => l.Stationen)
            .ToListAsync();

        return linien.Select(l => new LinieDetailDto(
            l.Id,
            l.Name,
            l.Stationen
                .OrderBy(s => s.Position)
                .Select(s => new StationInLinieDto(s.Id, s.Name, s.Position))
                .ToList()
        ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LinieDetailDto>> GetById(int id)
    {
        var linie = await db.Linien
            .Include(l => l.Stationen)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (linie is null) return NotFound();

        return new LinieDetailDto(
            linie.Id,
            linie.Name,
            linie.Stationen
                .OrderBy(s => s.Position)
                .Select(s => new StationInLinieDto(s.Id, s.Name, s.Position))
                .ToList()
        );
    }

    [HttpPost]
    public async Task<ActionResult<LinieDetailDto>> Create(LinieCreateDto dto)
    {
        var linie = new Linie { Name = dto.Name };
        db.Linien.Add(linie);
        await db.SaveChangesAsync();
        var result = new LinieDetailDto(linie.Id, linie.Name, []);
        return CreatedAtAction(nameof(GetById), new { id = linie.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LinieUpdateDto dto)
    {
        var linie = await db.Linien.FindAsync(id);
        if (linie is null) return NotFound();
        linie.Name = dto.Name;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var linie = await db.Linien.FindAsync(id);
        if (linie is null) return NotFound();
        db.Linien.Remove(linie);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
