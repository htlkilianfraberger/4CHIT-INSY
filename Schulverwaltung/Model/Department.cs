using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Departments")]
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public int SchoolId { get; set; }
    public School School { get; set; } = null!;

}