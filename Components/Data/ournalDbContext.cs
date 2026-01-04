using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Data;

public class JournalDbContext : DbContext
{
    public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options) { }

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Mood> Moods => Set<Mood>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EntryMood> EntryMoods => Set<EntryMood>();
    public DbSet<EntryTag> EntryTags => Set<EntryTag>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique: one journal entry per day
        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.EntryDate)
            .IsUnique();

        // DateOnly mapping (SQLite-safe) - store as TEXT "yyyy-MM-dd"
        modelBuilder.Entity<JournalEntry>()
            .Property(e => e.EntryDate)
            .HasConversion(
                d => d.ToString("yyyy-MM-dd"),
                s => DateOnly.ParseExact(s, "yyyy-MM-dd")
            );

        // EntryMood composite key
        modelBuilder.Entity<EntryMood>()
            .HasKey(em => new { em.JournalEntryId, em.MoodId });

        modelBuilder.Entity<EntryMood>()
            .HasOne(em => em.JournalEntry)
            .WithMany(e => e.EntryMoods)
            .HasForeignKey(em => em.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntryMood>()
            .HasOne(em => em.Mood)
            .WithMany(m => m.EntryMoods)
            .HasForeignKey(em => em.MoodId)
            .OnDelete(DeleteBehavior.Restrict);

        // EntryTag composite key
        modelBuilder.Entity<EntryTag>()
            .HasKey(et => new { et.JournalEntryId, et.TagId });

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.JournalEntry)
            .WithMany(e => e.EntryTags)
            .HasForeignKey(et => et.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.Tag)
            .WithMany(t => t.EntryTags)
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.Restrict);

        // Settings single row
        modelBuilder.Entity<AppSettings>()
            .HasKey(s => s.Id);
    }
}

