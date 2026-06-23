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

    // Rota: DELETE /api/missoes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarMissao(int id)
    {
        var missao = await _context.Missoes.FindAsync(id);
        if (missao == null) return NotFound("Missão não encontrada.");

        _context.Missoes.Remove(missao);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Missão removida do mural com sucesso." });
    }

    [HttpPost("{missaoId}/aceitar/{personagemId}")]
    public async Task<IActionResult> AceitarMissao(int missaoId, int personagemId)
    {
        var missao = await _context.Missoes.FindAsync(missaoId);
        var personagem = await _context.Personagens.FindAsync(personagemId);

        if (missao == null || personagem == null) return NotFound("Alvo não encontrado.");
        if (missao.Status != "Disponivel") return BadRequest("Esta missão não está disponível.");

        // 1. Contar quantos personagens totais existem na guilda para aplicar o bônus
        int totalPersonagens = await _context.Personagens.CountAsync();

        // 2. Calcular a redução (Dividir por 2 para cada personagem cadastrado)
        // Math.Pow(2, totalPersonagens) calcula 2 elevado ao número de personagens
        double divisor = Math.Pow(2, totalPersonagens);
        int tempoReduzidoSegundos = (int)Math.Ceiling(missao.TempoSegundosBase / divisor);

        // Garantir um tempo mínimo de pelo menos 2 segundos para o jogo não quebrar
        if (tempoReduzidoSegundos < 2) tempoReduzidoSegundos = 2;

        // 3. Atualizar a missão com o novo tempo dinâmico
        missao.Status = "Em Andamento";
        missao.PersonagemId = personagemId;
        missao.DataTermino = DateTime.Now.AddSeconds(tempoReduzidoSegundos);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = $"⚔️ {personagem.Nome} marchou! Graças aos {totalPersonagens} heróis da guilda, o tempo foi reduzido de {missao.TempoSegundosBase}s para apenas {tempoReduzidoSegundos}s!"
        });
    }

    // POST: api/missoes/{missaoId}/finalizar
    [HttpPost("{missaoId}/finalizar")]
    public async Task<IActionResult> FinalizarMissao(int missaoId)
    {
        var missao = await _context.Missoes.FindAsync(missaoId);

        if (missao == null) return NotFound("Missão não encontrada.");

        // === DIAGNÓSTICO MELHORADO ===
        if (missao.Status != "Em Andamento")
            return BadRequest($"Esta missão não pode ser finalizada porque o status atual é '{missao.Status}' e deveria ser 'Em Andamento'.");

        if (!missao.PersonagemId.HasValue)
            return BadRequest("Esta missão não possui nenhum herói vinculado a ela.");

        // Ajustamos para DateTime.Now (Horário local do seu PC) para bater com o navegador
        if (DateTime.Now < missao.DataTermino)
        {
            var segundosRestantes = (missao.DataTermino.Value - DateTime.Now).TotalSeconds;
            return BadRequest($"Os heróis ainda estão lutando! Aguarde mais {Math.Round(segundosRestantes)} segundos.");
        }

        var personagem = await _context.Personagens.FindAsync(missao.PersonagemId.Value);

        if (personagem == null)
        {
            missao.Status = "Disponivel";
            missao.PersonagemId = null;
            missao.DataTermino = null;
            await _context.SaveChangesAsync();
            return BadRequest("O herói faleceu misteriosamente antes de terminar. Status resetado.");
        }

        // APLICAR REGRA DE NEGÓCIO DE SOBREVIVÊNCIA
        if (personagem.Nivel < missao.nivelMinimo)
        {
            _context.Personagens.Remove(personagem);
            missao.Status = "Disponivel";
            missao.PersonagemId = null;
            missao.DataTermino = null;

            await _context.SaveChangesAsync();
            return Ok(new { resultado = "Morte", mensagem = $"💀 Tragédia! {personagem.Nome} não tinha o nível {missao.nivelMinimo} necessário e faleceu em combate!" });
        }
        else
        {
            personagem.SaldoOuro += missao.RecompensaOuro;
            personagem.Nivel += 1;
            missao.Status = "Concluida";

            await _context.SaveChangesAsync();
            return Ok(new { resultado = "Sucesso", mensagem = $"🏆 Vitória! {personagem.Nome} completou a quest, ganhou 💰 {missao.RecompensaOuro} PO e subiu para o nível {personagem.Nivel}!" });
        }
    }
}