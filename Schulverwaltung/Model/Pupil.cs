using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Pupils")]
public class Pupil
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public int? ClassId { get; set; }
    public Class? Class { get; set; }
}