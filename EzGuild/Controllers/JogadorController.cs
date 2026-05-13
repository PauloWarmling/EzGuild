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

        [HttpPost]
        public async Task<ActionResult<Jogador>> CriarJogador(Jogador jogador)
        {
            // Adiciona o jogador na memória do banco
            _context.Jogadores.Add(jogador);

            // Salva de fato no arquivo SQLite
            await _context.SaveChangesAsync();

            // Retorna o código 201 (Criado com sucesso) e mostra os dados salvos
            return CreatedAtAction(nameof(BuscarJogadorPorId), new { id = jogador.Id }, jogador);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jogador>>> ListarJogadores()
        {
            // Busca todos os jogadores no banco e transforma em uma lista
            return await _context.Jogadores.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Jogador>> BuscarJogadorPorId(int id)
        {
            var jogador = await _context.Jogadores.FindAsync(id);

            // Se não achar ninguém com esse ID, avisa que não encontrou (Erro 404)
            if (jogador == null)
            {
                return NotFound("Jogador não encontrado na guilda.");
            }

            return jogador;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarJogador(int id, Jogador jogador)
        {
            // Verifica se o ID passado na URL é o mesmo ID do jogador enviado no corpo (Body)
            if (id != jogador.Id) return BadRequest("Os IDs não coincidem.");

            // Avisa o banco que este jogador foi modificado
            _context.Entry(jogador).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent(); // Retorna código 204 (Sucesso, mas sem conteúdo extra para mostrar)
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarJogador(int id)
        {
            var jogador = await _context.Jogadores.FindAsync(id);
            if (jogador == null) return NotFound("Jogador não encontrado.");

            // Remove da memória e salva
            _context.Jogadores.Remove(jogador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
