using System.Text.Json.Serialization;

namespace Model;
[JsonDerivedType(typeof(Dog), typeDiscriminator: "dog")]
[JsonDerivedType(typeof(Bird), typeDiscriminator: "bird")]
public abstract class Animal //Müss für TPC abstract sein
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