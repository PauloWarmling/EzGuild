namespace EzGuild.Models
{
    public class Missao
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public decimal RecompensaOuro { get; set; }
        public int nivelMinimo { get; set; }
        public string Status { get; set; } = "Disponível";

        public List<Personagem> Personagens { get; set; } = new();

    }
}
