using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Gamertag { get; set; }

    public int TeamId { get; set; }

    [ForeignKey(nameof(TeamId))]
    [InverseProperty(nameof(Team.Players))]
    public Team Team { get; set; } = null!;
}
