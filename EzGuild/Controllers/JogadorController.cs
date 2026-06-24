using EzGuild.Data;
using EzGuild.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzGuild.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogadorController : ControllerBase
    {
        private readonly EzGuildDbContext _context;

        public JogadorController(EzGuildDbContext context)
        {
            _context = context;
        }

        // Rota: POST /api/jogador
        [HttpPost]
        public async Task<ActionResult<Jogador>> CriarJogador(Jogador jogador)
        {
            _context.Jogadores.Add(jogador);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarJogadorPorId), new { id = jogador.Id }, jogador);
        }

        // Rota: GET /api/jogador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jogador>>> ListarJogadores()
        {
            return await _context.Jogadores
                .Include(j => j.Personagens)
                .ToListAsync();
        }

        // Rota: GET /api/jogador/jogadorid
        [HttpGet("{id}")]
        public async Task<ActionResult<Jogador>> BuscarJogadorPorId(int id)
        {
            var jogador = await _context.Jogadores.FindAsync(id);

            if (jogador == null)
            {
                return NotFound("Jogador não encontrado na guilda.");
            }

            return jogador;
        }

        // Rota: PUT /api/jogador/jogadorid
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarJogador(int id, Jogador jogador)
        {
            if (id != jogador.Id) return BadRequest("Os IDs não coincidem.");

            _context.Entry(jogador).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Rota: DELETE /api/jogador/jogadorid
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarJogador(int id)
        {
            var jogador = await _context.Jogadores.FindAsync(id);
            if (jogador == null) return NotFound("Jogador não encontrado.");

            _context.Jogadores.Remove(jogador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
