using System.ComponentModel.DataAnnotations;

namespace MYMAUIAPP.Components.Model;

public class JournalEntry
{
    public int Id { get; set; }

    // One entry per day enforced via unique index + service checks
    [Required]
    public DateOnly EntryDate { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required]
    public string ContentMarkdown { get; set; } = "";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int WordCount { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<EntryMood> EntryMoods { get; set; } = new();
    public List<EntryTag> EntryTags { get; set; } = new();
}
