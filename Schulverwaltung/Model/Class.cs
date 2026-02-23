using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Classes")]
public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public List<Pupil> Pupils { get; set; } = null!;
}