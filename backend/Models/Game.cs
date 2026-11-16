using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Game
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public string? Description { get; set; }

    [InverseProperty(nameof(Tournament.Game))]
    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
