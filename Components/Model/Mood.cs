using System.ComponentModel.DataAnnotations;
using MYMAUIAPP.Components.Model.Enums;

namespace MYMAUIAPP.Components.Model;

public class Mood
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = "";

    public MoodGroup MoodGroup { get; set; }

    public List<EntryMood> EntryMoods { get; set; } = new();
}
