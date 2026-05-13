using Microsoft.EntityFrameworkCore;
using EzGuild.Models;

namespace EzGuild.Data
{
    public class EzGuildDbContext : DbContext
    {
        public EzGuildDbContext(DbContextOptions<EzGuildDbContext> options) : base(options) { }
        public DbSet<Jogador> Jogadores { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<Missao> Missoes { get; set; }
        
    }
}
