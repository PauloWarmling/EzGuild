using EzGuild.Data;
using EzGuild.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzGuild.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MissoesController : ControllerBase
{
    private readonly EzGuildDbContext _context;

    // O Construtor injeta o banco de dados (mesmo nome da classe!)
    public MissoesController(EzGuildDbContext context)
    {
        _context = context;
    }

    // 1. CREATE: Adiciona uma nova missão ao quadro da taverna
    // Rota: POST /api/missoes
    [HttpPost]
    public async Task<ActionResult<Missao>> CriarMissao(Missao missao)
    {
        // Força o status inicial para "Disponivel" se vier vazio
        if (string.IsNullOrEmpty(missao.Status))
        {
            missao.Status = "Disponivel";
        }

        _context.Missoes.Add(missao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarMissaoPorId), new { id = missao.Id }, missao);
    }

    // 2. READ: Lista todas as missões cadastradas
    // Rota: GET /api/missoes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Missao>>> ListarMissoes()
    {
        return await _context.Missoes
            .Include(m => m.Personagens)
            .ToListAsync();
    }

    // 3. READ: Busca os detalhes de uma missão específica pelo ID
    // Rota: GET /api/missoes/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Missao>> BuscarMissaoPorId(int id)
    {
        var missao = await _context.Missoes.FindAsync(id);

        if (missao == null)
        {
            return NotFound("Essa missão não existe no quadro da guilda.");
        }

        return missao;
    }

    // 4. UPDATE: Atualiza os dados de uma missão (ex: mudar o status para "Concluida")
    // Rota: PUT /api/missoes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarMissao(int id, Missao missao)
    {
        if (id != missao.Id) return BadRequest("Os IDs não coincidem.");

        _context.Entry(missao).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 5. DELETE: Remove uma missão do quadro
    // Rota: DELETE /api/missoes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMissao(int id)
    {
        var missao = await _context.Missoes.FindAsync(id);
        if (missao == null) return NotFound("Missão não encontrada.");

        _context.Missoes.Remove(missao);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{missaoId}/aceitar/{personagemId}")]
    public async Task<IActionResult> AceitarMissao(int missaoId, int personagemId)
    {
        var missao = await _context.Missoes
            .Include(m => m.Personagens) 
            .FirstOrDefaultAsync(m => m.Id == missaoId);

        var personagem = await _context.Personagens.FindAsync(personagemId);

        if (missao == null) return NotFound("Esta missão não existe no quadro.");
        if (personagem == null) return NotFound("Este herói não foi encontrado na guilda.");

        if (missao.Personagens.Any(p => p.Id == personagemId))
        {
            return BadRequest("Este herói já aceitou esta missão anteriormente!");
        }

        missao.Personagens.Add(personagem);
        missao.Status = "Em Andamento";

        await _context.SaveChangesAsync();

        return Ok($"Sucesso! O herói {personagem.Nome} aceitou o contrato: '{missao.Titulo}'.");
    }
}