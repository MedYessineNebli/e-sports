using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TournamentRegistration
{
    [Key]
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int TeamId { get; set; }

    public DateTime RegisteredAt { get; set; }

    [ForeignKey(nameof(TournamentId))]
    [InverseProperty(nameof(Tournament.TournamentRegistrations))]
    public Tournament Tournament { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    [InverseProperty(nameof(Team.TournamentRegistrations))]
    public Team Team { get; set; } = null!;
}
