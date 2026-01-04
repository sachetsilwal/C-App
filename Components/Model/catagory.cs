using System.ComponentModel.DataAnnotations;

namespace MYMAUIAPP.Components.Model;

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = "";

    public List<JournalEntry> Entries { get; set; } = new();
}
