using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Model;

// --- Enums ---

public enum WeekDay { Mo = 1, Di, Mi, Do, Fr }

public enum SchoolClassEnum { CHIT = 1, CHIT2, CHIT3, CHIT4, CHIT5 }

public enum TeacherEnum { ALLI, MACO, NIGI, LEYV, WART, BRUN, KUBI, JAGE, WIEN, ELSH, WINN, KIEN, HAUP, HAUL }

public enum SubjectEnum { AM, D, E1, INSY, RK, SEW, SYTD, GGPH, GGPG, NW2P, NW2C, DSAI, WIR, ITPL, ITPP, BESP, SYTI, SYTS }

// Zeitfenster laut deinem Plan
public enum LessonHour 
{ 
    H1_0745 = 1, // 07:45 - 08:35
    H2_0835 = 2, // 08:35 - 09:25
    H3_0940 = 3, // 09:40 - 10:30
    H4_1030 = 4, // 10:30 - 11:20
    H5_1125 = 5, // 11:25 - 12:15
    H6_1215 = 6, // 12:15 - 13:05
    H7_1305 = 7, // 13:05 - 13:55
    H8_1355 = 8, // 13:55 - 14:45
    H9_1455 = 9,  // 14:55 - 15:45
    H10_1545 = 10, // 15:45 - 16:35
    
}

// --- Entities ---

public class Teacher {
    public int Id { get; set; }
    public TeacherEnum Abbr { get; set; } // Geändert auf Enum
    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}

public class Subject {
    public int Id { get; set; }
    public SubjectEnum Desc { get; set; } // Geändert auf Enum
}

public class SchoolClass {
    public int Id { get; set; }
    public SchoolClassEnum Abbr { get; set; } // Geändert auf Enum
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
    public LessonHour Hour { get; set; } // Geändert auf Enum
    
    [JsonIgnore] public virtual TeacherSubject? TeacherSubject { get; set; }
    [JsonIgnore] public virtual ClassSubject? ClassSubject { get; set; }
}