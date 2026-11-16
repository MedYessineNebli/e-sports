using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Tournament
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public required string Name { get; set; }

    public int GameId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [ForeignKey(nameof(GameId))]
    [InverseProperty(nameof(Game.Tournaments))]
    public Game Game { get; set; } = null!;

    [InverseProperty(nameof(TournamentRegistration.Tournament))]
    public ICollection<TournamentRegistration> TournamentRegistrations { get; set; } = new List<TournamentRegistration>();

    [InverseProperty(nameof(Match.Tournament))]
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
