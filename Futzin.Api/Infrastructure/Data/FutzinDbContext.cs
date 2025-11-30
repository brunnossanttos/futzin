using Microsoft.EntityFrameworkCore;
using Futzin.Api.Domain.Entities;

namespace Futzin.Api.Infrastructure.Data;

public class FutzinDbContext : DbContext
{
    public FutzinDbContext(DbContextOptions<FutzinDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Pelada> Peladas { get; set; }
    public DbSet<PeladaParticipant> PeladaParticipants { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Goal> Goals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Pelada configuration
        modelBuilder.Entity<Pelada>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.CreatedBy)
                  .WithMany(u => u.PeladasCreated)
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PeladaParticipant configuration
        modelBuilder.Entity<PeladaParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Pelada)
                  .WithMany(p => p.Participants)
                  .HasForeignKey(e => e.PeladaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Participations)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Team)
                  .WithMany(t => t.Players)
                  .HasForeignKey(e => e.TeamId)
                  .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => new { e.PeladaId, e.UserId }).IsUnique();
        });

        // Team configuration
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
            
            entity.HasOne(e => e.Pelada)
                  .WithMany(p => p.Teams)
                  .HasForeignKey(e => e.PeladaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Goal configuration
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Pelada)
                  .WithMany(p => p.Goals)
                  .HasForeignKey(e => e.PeladaId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Scorer)
                  .WithMany(u => u.Goals)
                  .HasForeignKey(e => e.ScorerId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Team)
                  .WithMany(t => t.Goals)
                  .HasForeignKey(e => e.TeamId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
