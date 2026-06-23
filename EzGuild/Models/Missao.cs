using System;
using System.Collections.Generic;

namespace EzGuild.Models
{
    public class Missao
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty; // Adicionado para bater com o formulário

        // Alterado para int para bater com o SaldoOuro do Personagem
        public decimal RecompensaOuro { get; set; }

        public int nivelMinimo { get; set; }
        public string Status { get; set; } = "Disponível";

        // === CAMPOS OBRIGATÓRIOS PARA O CRONÔMETRO E REDUÇÃO ===
        public int TempoSegundosBase { get; set; }
        public DateTime? DataTermino { get; set; }
        public int? PersonagemId { get; set; }

        // Mantendo a tua lista de relacionamento caso queiras usar no futuro
        public List<Personagem> Personagens { get; set; } = new();
    }
}