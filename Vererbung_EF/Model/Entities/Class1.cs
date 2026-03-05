namespace Model;

public abstract class Animal
{
    public int Id { get; set; }
    public DateTime Recorded { get; set; }
}

public class Dog : Animal
{
    public string Breed { get; set; }
}

public class Bird : Animal
{
    public string Wingspan { get; set; }
}