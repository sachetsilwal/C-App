namespace MYMAUIAPP.Components.Model;

public class EntryMood
{
    public int JournalEntryId { get; set; }
    public JournalEntry JournalEntry { get; set; } = null!;

    public int MoodId { get; set; }
    public Mood Mood { get; set; } = null!;

    // Exactly one Primary per entry; Secondary can be 0..2
    public bool IsPrimary { get; set; }
}
