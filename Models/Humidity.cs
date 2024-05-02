using System.ComponentModel.DataAnnotations;

namespace SmartAsthmaAssistane.Models;

public class Humidity
{
    [Key]
    public int Id { get; set; }

    public float Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime RecordedOn { get; set; }
}
