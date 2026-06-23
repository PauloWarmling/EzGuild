using EzGuild.Data;
using EzGuild.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzGuild.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonagensController : ControllerBase
{
    private readonly EzGuildDbContext _context;

    public PersonagensController(EzGuildDbContext context)
    {
        _context = context;
    }

    // Rota: POST /api/personagens
    [HttpPost]
    public async Task<ActionResult<Personagem>> CriarPersonagem(Personagem personagem)
    {
        var jogadorExiste = await _context.Jogadores.AnyAsync(j => j.Id == personagem.JogadorId);
        
        if (!jogadorExiste)
        {
            return BadRequest("Não é possível criar o personagem. O JogadorId informado não existe.");
        }

        _context.Personagens.Add(personagem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPersonagemPorId), new { id = personagem.Id }, personagem);
    }

    // Rota: GET /api/personagens
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Personagem>>> ListarPersonagens()
    {
        return await _context.Personagens
            .Include(p => p.Jogador)
            .ToListAsync();
    }

    // Rota: GET /api/personagens/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Personagem>> BuscarPersonagemPorId(int id)
    {
        var personagem = await _context.Personagens
            .Include(p => p.Jogador)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (personagem == null)
        {
            return NotFound("Herói não encontrado nesta guilda.");
        }

        return personagem;
    }

    // Rota: PUT /api/personagens/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPersonagem(int id, Personagem personagem)
    {
        if (id != personagem.Id) return BadRequest("Os IDs não coincidem.");

        _context.Entry(personagem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // Rota: DELETE /api/personagens/{id}

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarPersonagem(int id)
    {
        var personagem = await _context.Personagens.FindAsync(id);
        if (personagem == null) return NotFound("Herói não encontrado.");

        _context.Personagens.Remove(personagem);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Herói removido da guilda com sucesso." });
    }
}