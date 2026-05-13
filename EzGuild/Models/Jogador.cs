namespace EzGuild.Models
{
    public class Jogador
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public DateTime DataCadastro { get; set; }
        public string StatusConta { get; set; } = "Ativa";

        public List<Personagem> Personagens { get; set; } = new();
    }
}
