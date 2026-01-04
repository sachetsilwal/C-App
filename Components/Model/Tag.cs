using System.ComponentModel.DataAnnotations;

namespace MYMAUIAPP.Components.Model;

public class Tag
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = "";

    public bool IsPrebuilt { get; set; }

    public List<EntryTag> EntryTags { get; set; } = new();
}
