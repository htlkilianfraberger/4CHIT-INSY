namespace Model;

public abstract class Animal
{
    public int Id { get; set; }
    public DateTime Recorded { get; set; }
}

public class Dog : Animal, IHasId
{
    public string Breed { get; set; }
}

public class Bird : Animal, IHasId
{
    public string Wingspan { get; set; }
}