using System.ComponentModel.DataAnnotations.Schema;

namespace Model;
[Table("Examples")]
public class Example : IHasId
{
    public int Id { get; set; }
    public int Value1 { get; set; }
    public int Value2 { get; set; }
}