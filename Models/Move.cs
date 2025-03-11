using System.ComponentModel.DataAnnotations;

namespace LegendaryPokedex.Models
{
    public class Move
    {
        [Key]
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Power { get; set; }
        public int PP { get; set; }
        public int Accuracy { get; set; }
        public required string Description { get; set; }
    }
}