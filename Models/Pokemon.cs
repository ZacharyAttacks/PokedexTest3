namespace LegendaryPokedex.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Typing { get; set; }
        public string Description { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public string? Move1 { get; set; } = "";
        public string? Move2 { get; set; } = "";
        public string? Move3 { get; set; } = "";
        public string? Move4 { get; set; } = "";
        public bool IsFavorite { get; set; }
        public string? Form { get; set; }
    }
}
