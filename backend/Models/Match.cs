using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Match
{
    [Key]
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int TeamAId { get; set; }

    public int TeamBId { get; set; }

    public int? WinnerTeamId { get; set; }

    public int ScoreTeamA { get; set; }

    public int ScoreTeamB { get; set; }

    public DateTime ScheduledAt { get; set; }

    public DateTime? PlayedAt { get; set; }

    [ForeignKey(nameof(TournamentId))]
    [InverseProperty(nameof(Tournament.Matches))]
    public Tournament Tournament { get; set; } = null!;

    [ForeignKey(nameof(TeamAId))]
    [InverseProperty(nameof(Team.TeamAMatches))]
    public Team TeamA { get; set; } = null!;

    [ForeignKey(nameof(TeamBId))]
    [InverseProperty(nameof(Team.TeamBMatches))]
    public Team TeamB { get; set; } = null!;

    [ForeignKey(nameof(WinnerTeamId))]
    [InverseProperty(nameof(Team.WonMatches))]
    public Team? WinnerTeam { get; set; }
}
