namespace EzGuild.Models
{
    public class Personagem
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Classe { get; set; }
        public int Nivel { get; set; }

        public Decimal SaldoOuro { get; set; } = 0;

        public int JogadorId { get; set; }
        public Jogador? Jogador { get; set; }

        public List<Missao> Missoes { get; set; } = new();
    }
}
