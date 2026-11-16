using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public class ApplicationContext(DbContextOptions<ApplicationContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentRegistration> TournamentRegistrations => Set<TournamentRegistration>();
    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(game => game.Name).HasMaxLength(100).IsRequired();
            entity.Property(game => game.Description).HasMaxLength(1000);
            entity.HasIndex(game => game.Name).IsUnique();
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(team => team.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(team => team.Name).IsUnique();
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(player => player.Gamertag).HasMaxLength(100).IsRequired();
            entity.HasOne(player => player.Team)
                .WithMany(team => team.Players)
                .HasForeignKey(player => player.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.Property(tournament => tournament.Name).HasMaxLength(150).IsRequired();
            entity.HasOne(tournament => tournament.Game)
                .WithMany(game => game.Tournaments)
                .HasForeignKey(tournament => tournament.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TournamentRegistration>(entity =>
        {
            entity.HasIndex(registration => new
            {
                registration.TournamentId,
                registration.TeamId
            }).IsUnique();
            entity.HasOne(registration => registration.Tournament)
                .WithMany(tournament => tournament.TournamentRegistrations)
                .HasForeignKey(registration => registration.TournamentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(registration => registration.Team)
                .WithMany(team => team.TournamentRegistrations)
                .HasForeignKey(registration => registration.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasOne(match => match.Tournament)
                .WithMany(tournament => tournament.Matches)
                .HasForeignKey(match => match.TournamentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(match => match.TeamA)
                .WithMany(team => team.TeamAMatches)
                .HasForeignKey(match => match.TeamAId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(match => match.TeamB)
                .WithMany(team => team.TeamBMatches)
                .HasForeignKey(match => match.TeamBId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(match => match.WinnerTeam)
                .WithMany(team => team.WonMatches)
                .HasForeignKey(match => match.WinnerTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Game>().HasData(
            new Game { Id = 1, Name = "League of Legends", Description = "MOBA compétitif" },
            new Game { Id = 2, Name = "Counter-Strike 2", Description = "FPS tactique" },
            new Game { Id = 3, Name = "Rocket League", Description = "Football automobile" });

        modelBuilder.Entity<Team>().HasData(
            new Team { Id = 1, Name = "Atlas Esports" },
            new Team { Id = 2, Name = "Nova Gaming" },
            new Team { Id = 3, Name = "Phoenix Squad" },
            new Team { Id = 4, Name = "Titan United" });

        modelBuilder.Entity<Player>().HasData(
            new Player { Id = 1, Gamertag = "AtlasOne", TeamId = 1 },
            new Player { Id = 2, Gamertag = "AtlasTwo", TeamId = 1 },
            new Player { Id = 3, Gamertag = "NovaOne", TeamId = 2 },
            new Player { Id = 4, Gamertag = "NovaTwo", TeamId = 2 },
            new Player { Id = 5, Gamertag = "PhoenixOne", TeamId = 3 },
            new Player { Id = 6, Gamertag = "TitanOne", TeamId = 4 });

        modelBuilder.Entity<Tournament>().HasData(
            new Tournament
            {
                Id = 1,
                Name = "Summer Championship",
                GameId = 1,
                StartDate = new DateTime(2026, 6, 1),
                EndDate = new DateTime(2026, 6, 30)
            },
            new Tournament
            {
                Id = 2,
                Name = "Winter Cup",
                GameId = 2,
                StartDate = new DateTime(2026, 12, 1),
                EndDate = new DateTime(2026, 12, 20)
            });

        modelBuilder.Entity<TournamentRegistration>().HasData(
            new TournamentRegistration { Id = 1, TournamentId = 1, TeamId = 1, RegisteredAt = new DateTime(2026, 5, 1) },
            new TournamentRegistration { Id = 2, TournamentId = 1, TeamId = 2, RegisteredAt = new DateTime(2026, 5, 2) },
            new TournamentRegistration { Id = 3, TournamentId = 1, TeamId = 3, RegisteredAt = new DateTime(2026, 5, 3) },
            new TournamentRegistration { Id = 4, TournamentId = 2, TeamId = 2, RegisteredAt = new DateTime(2026, 11, 1) },
            new TournamentRegistration { Id = 5, TournamentId = 2, TeamId = 4, RegisteredAt = new DateTime(2026, 11, 2) });

        modelBuilder.Entity<Match>().HasData(
            new Match
            {
                Id = 1,
                TournamentId = 1,
                TeamAId = 1,
                TeamBId = 2,
                WinnerTeamId = 1,
                ScoreTeamA = 2,
                ScoreTeamB = 1,
                ScheduledAt = new DateTime(2026, 6, 10, 18, 0, 0),
                PlayedAt = new DateTime(2026, 6, 10, 18, 10, 0)
            },
            new Match
            {
                Id = 2,
                TournamentId = 2,
                TeamAId = 2,
                TeamBId = 4,
                ScoreTeamA = 0,
                ScoreTeamB = 0,
                ScheduledAt = new DateTime(2026, 12, 5, 18, 0, 0)
            });
    }
}
