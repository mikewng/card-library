using card_library.Core.Application.Models;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace card_library.Core.Infrastructure
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Deck> Decks => Set<Deck>();
        public DbSet<DeckTag> DeckTags => Set<DeckTag>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<CardSection> CardSections => Set<CardSection>();
        public DbSet<CardTag> CardTags => Set<CardTag>();
        public DbSet<User> Users => Set<User>();
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
            => base.SaveChangesAsync(ct);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(e =>
            {
                e.ToTable("Games");
                e.HasKey(g => g.Id);

                e.Property(g => g.GameName);
                e.Property(g => g.Description);
                e.Property(g => g.ImageRefUrl);

                e.HasMany(g => g.Decks)
                 .WithOne(d => d.Game!)
                 .HasForeignKey(c => c.GameId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Deck>(e =>
            {
                e.ToTable("Decks");
                e.HasKey(d => d.Id);
                e.Property(d => d.Name).IsRequired().HasMaxLength(100);
                e.Property(d => d.Description);
                e.Property(d => d.ImageRefUrl);

                e.HasMany(d => d.Cards)
                 .WithOne(c => c.Deck!)
                 .HasForeignKey(c => c.DeckId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(d => d.DeckTags)
                 .WithOne(dt => dt.Deck!)
                 .HasForeignKey(dt => dt.DeckId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeckTag>(e =>
            {
                e.ToTable("DeckTags");
                e.HasKey(c => c.Id);
                e.Property(c => c.Type);

                e.HasIndex(dt => new { dt.DeckId, dt.Type })
                .IsUnique();
            });

            modelBuilder.Entity<Card>(e =>
            {
                e.ToTable("Cards");
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(200);
                e.Property(c => c.HexCardColor);
                e.Property(c => c.ImageRefUrl);
                e.Property(c => c.IsRawCardImage);

                e.HasMany(c => c.CardSections)
                .WithOne(cs => cs.Card)
                .HasForeignKey(cs => cs.CardId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CardSection>(e =>
            {
                e.ToTable("CardSections");
                e.HasKey(c => c.Id);
                e.Property(c => c.TextField).HasMaxLength(200);
                e.Property(c => c.IsBolded);
                e.Property(c => c.IsItalicized);
                e.Property(c => c.TextSize);
                e.Property(c => c.Rank);
                e.Property(c => c.Position);
            });

            modelBuilder.Entity<CardTag>(e =>
            {
                e.ToTable("CardTags");
                e.HasKey(c => c.Id);
                e.Property(c => c.Type);

                e.HasIndex(ct => new { ct.CardId, ct.Type })
                .IsUnique();
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(u => u.Id);

                e.Property(u => u.Id).ValueGeneratedOnAdd();
                e.Property(u => u.Username).IsRequired().HasMaxLength(50);
                e.Property(u => u.Email).IsRequired().HasMaxLength(100);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                e.Property(u => u.CreatedAt).IsRequired();
                e.Property(u => u.LastLoginAt);

                e.HasIndex(u => u.Username).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
