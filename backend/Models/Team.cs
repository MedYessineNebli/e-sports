using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Team
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [InverseProperty(nameof(Player.Team))]
    public ICollection<Player> Players { get; set; } = new List<Player>();

    [InverseProperty(nameof(TournamentRegistration.Team))]
    public ICollection<TournamentRegistration> TournamentRegistrations { get; set; } = new List<TournamentRegistration>();

    [InverseProperty(nameof(Match.TeamA))]
    public ICollection<Match> TeamAMatches { get; set; } = new List<Match>();

    [InverseProperty(nameof(Match.TeamB))]
    public ICollection<Match> TeamBMatches { get; set; } = new List<Match>();

    [InverseProperty(nameof(Match.WinnerTeam))]
    public ICollection<Match> WonMatches { get; set; } = new List<Match>();
}
