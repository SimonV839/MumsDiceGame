using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfDbFirstConsole.Models;

public partial class MumsDiceGameContext : DbContext
{
    public MumsDiceGameContext()
    {
    }

    public MumsDiceGameContext(DbContextOptions<MumsDiceGameContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DiceGame> DiceGames { get; set; }

    public virtual DbSet<GameRequest> GameRequests { get; set; }

    public virtual DbSet<GameUser> GameUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local)\\SQLEXPRESS;database=MumsDiceGame;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiceGame>(entity =>
        {
            entity.HasKey(e => e.DiceGameId).HasName("PK__DiceGame__AF7E0E2D0DFE543E");

            entity.HasOne(d => d.Player1).WithMany(p => p.DiceGamePlayer1s)
                .HasForeignKey(d => d.Player1Id)
                .HasConstraintName("FK__DiceGames__Playe__3D5E1FD2");

            entity.HasOne(d => d.Player2).WithMany(p => p.DiceGamePlayer2s)
                .HasForeignKey(d => d.Player2Id)
                .HasConstraintName("FK__DiceGames__Playe__3E52440B");
        });

        modelBuilder.Entity<GameRequest>(entity =>
        {
            entity.HasKey(e => e.GameRequestId).HasName("PK__GameRequ__4315A49E9E60C89F");

            entity.Property(e => e.OpponentName)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.GameUser).WithMany(p => p.GameRequests)
                .HasForeignKey(d => d.GameUserId)
                .HasConstraintName("FK__GameReque__GameU__3A81B327");
        });

        modelBuilder.Entity<GameUser>(entity =>
        {
            entity.HasKey(e => e.GameUserId).HasName("PK__GameUser__50CF2544C119072C");

            entity.HasIndex(e => e.Name, "UQ__GameUser__737584F6010E6721").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.HasOne(d => d.DiceGame).WithMany(p => p.GameUsers)
                .HasForeignKey(d => d.DiceGameId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__GameUsers__DiceG__403A8C7D");

            entity.HasOne(d => d.GameRequest).WithMany(p => p.GameUsers)
                .HasForeignKey(d => d.GameRequestId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__GameUsers__GameR__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
