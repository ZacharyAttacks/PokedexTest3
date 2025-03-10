using LegendaryPokedex.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendaryPokedex.Data
{
    public class PokedexContext : DbContext
    {
        public PokedexContext(DbContextOptions<PokedexContext> options) : base(options) { }

        public DbSet<Pokemon> Pokemons { get; set; }
    }
}
