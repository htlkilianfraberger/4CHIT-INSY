using System.ComponentModel.DataAnnotations.Schema;
 
 namespace Model;
 
 [Table("Schools")]
 public class School
 {
     public int Id { get; set; }
     public string Name { get; set; } = null!;
     
     public List<Department> Departments { get; set; } = null!;
 }