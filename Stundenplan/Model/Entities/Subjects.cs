namespace Model;

public class Subjects
{
    public int Id { get; set; }
    public string Description { get; set; }
    public List<Classes> C { get; set; }
}