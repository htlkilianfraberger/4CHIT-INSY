using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model;

// --- Logik-Enums bleiben (für das Grid-Layout sinnvoll) ---
public enum WeekDay { Mo = 1, Di, Mi, Do, Fr }

public enum LessonHour 
{ 
    [Description("1. 07:45 - 08:35")]
    H1_0745 = 1, 

    [Description("2. 08:35 - 09:25")]
    H2_0835 = 2, 

    [Description("3. 09:40 - 10:30")]
    H3_0940 = 3, 

    [Description("4. 10:30 - 11:20")]
    H4_1030 = 4, 

    [Description("5. 11:25 - 12:15")]
    H5_1125 = 5, 

    [Description("6. 12:15 - 13:05")]
    H6_1215 = 6, 

    [Description("7. 13:05 - 13:55")]
    H7_1305 = 7, 

    [Description("8. 13:55 - 14:45")]
    H8_1355 = 8, 

    [Description("9. 14:55 - 15:45")]
    H9_1455 = 9, 

    [Description("10. 15:45 - 16:35")]
    H10_1545 = 10
}

// --- Entities ohne Enums ---

public class Teacher {
    public int Id { get; set; }
    public string Abbr { get; set; } = string.Empty; // Direkt als String
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}

public class Subject {
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty; // Direkt als String
}

public class SchoolClass {
    public int Id { get; set; }
    public string Abbr { get; set; } = string.Empty; // Direkt als String
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
    public WeekDay WeekDay { get; set; }
    public LessonHour Hour { get; set; }
    
    [JsonIgnore] public virtual TeacherSubject? TeacherSubject { get; set; }
    [JsonIgnore] public virtual ClassSubject? ClassSubject { get; set; }
}