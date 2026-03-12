using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model;

public class Teacher {
    public int Id { get; set; }
    [MaxLength(20)] public string Abbr { get; set; } = string.Empty;
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}

public class Subject {
    public int Id { get; set; }
    [MaxLength(100)] public string Desc { get; set; } = string.Empty;
}

public class SchoolClass {
    public int Id { get; set; }
    [MaxLength(20)] public string Abbr { get; set; } = string.Empty;
    public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
}

public class TeacherSubject {
    public int Tid { get; set; }
    public virtual Teacher Teacher { get; set; } = null!;
    public int Sid { get; set; }
    public virtual Subject Subject { get; set; } = null!;
}

public class ClassSubject {
    public int Cid { get; set; }
    public virtual SchoolClass SchoolClass { get; set; } = null!;
    public int Sid { get; set; }
    public virtual Subject Subject { get; set; } = null!;
}

public class Lesson {
    public int Id { get; set; }
    public int Tid { get; set; }
    public int Sid { get; set; }
    public int Cid { get; set; }
    [MaxLength(20)] public string? Day { get; set; }
    public int? Hour { get; set; }
    public virtual TeacherSubject TeacherSubject { get; set; } = null!;
    public virtual ClassSubject ClassSubject { get; set; } = null!;
}