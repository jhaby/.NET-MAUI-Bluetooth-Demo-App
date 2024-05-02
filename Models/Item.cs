using System.ComponentModel.DataAnnotations;

namespace SmartAsthmaAssistane.Models;

public class Item
{
    [Key]
    public int Id { get; set; }
    public DateTime RecordedOn { get; set; }
    public float Value { get; set; }
    public string Parameter { get; set; } = string.Empty;
}
