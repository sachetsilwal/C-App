using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Model.Enums;

namespace MYMAUIAPP.Components.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(JournalDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        // Ensure settings row exists
        if (!await db.Settings.AnyAsync())
        {
            db.Settings.Add(new AppSettings
            {
                Id = 1,
                ThemeMode = ThemeMode.Dark,
                IsLockEnabled = false,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // Seed moods (fixed set from spec)
        if (!await db.Moods.AnyAsync())
        {
            var moods = new List<Mood>
            {
                // Positive
                new() { Name="Happy", MoodGroup=MoodGroup.Positive },
                new() { Name="Excited", MoodGroup=MoodGroup.Positive },
                new() { Name="Relaxed", MoodGroup=MoodGroup.Positive },
                new() { Name="Grateful", MoodGroup=MoodGroup.Positive },
                new() { Name="Confident", MoodGroup=MoodGroup.Positive },

                // Neutral
                new() { Name="Calm", MoodGroup=MoodGroup.Neutral },
                new() { Name="Thoughtful", MoodGroup=MoodGroup.Neutral },
                new() { Name="Curious", MoodGroup=MoodGroup.Neutral },
                new() { Name="Nostalgic", MoodGroup=MoodGroup.Neutral },
                new() { Name="Bored", MoodGroup=MoodGroup.Neutral },

                // Negative
                new() { Name="Sad", MoodGroup=MoodGroup.Negative },
                new() { Name="Angry", MoodGroup=MoodGroup.Negative },
                new() { Name="Stressed", MoodGroup=MoodGroup.Negative },
                new() { Name="Lonely", MoodGroup=MoodGroup.Negative },
                new() { Name="Anxious", MoodGroup=MoodGroup.Negative }
            };

            db.Moods.AddRange(moods);
        }

        // Seed tags (prebuilt list from spec)
        if (!await db.Tags.AnyAsync())
        {
            var prebuilt = new[]
            {
                "Work","Career","Studies","Family","Friends","Relationships","Health","Fitness",
                "Personal Growth","Self-care","Hobbies","Travel","Nature","Finance","Spirituality",
                "Birthday","Holiday","Vacation","Celebration","Exercise","Reading","Writing","Cooking",
                "Meditation","Yoga","Music","Shopping","Parenting","Projects","Planning","Reflection"
            };

            db.Tags.AddRange(prebuilt.Select(t => new Tag { Name = t, IsPrebuilt = true }));
        }

        // Seed some categories (not fixed in spec, but category feature exists)
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "General" },
                new Category { Name = "Work & Studies" },
                new Category { Name = "Health" },
                new Category { Name = "Travel & Memories" }
            );
        }

        await db.SaveChangesAsync();
    }
}
