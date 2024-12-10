using demo_project.Models;

namespace demo_project.Controllers.DataPkg;

public class Data
{
    private static List<Group> _groups = [
        new Group() { Id = 1, Name = "Group 1"},
        new Group() { Id = 2, Name = "Group two"},
        new Group() { Id = 37, Name = "373737"}
    ];

    private static IEnumerable<Student> _students = [
        new Student(){
            Id = 1,
            FirstName = "Mike",
            LastName = "Tailor",
            DateOfBirth = new DateOnly(1992, 12, 1),
            Email = "mt@random-tu.de",
            GroupId = 1,
        },
        new Student(){
            Id = 2,
            FirstName = "James",
            LastName = "Smith",
            DateOfBirth = new DateOnly(502, 12, 1),
            Email = "js@random-tu.de",
            GroupId = 1,
        },
        new Student(){
            Id = 3,
            FirstName = "Catherin",
            LastName = "Tailor",
            DateOfBirth = new DateOnly(2023, 12, 1),
            Email = "ct@random-tu.de",
            GroupId = 2,
        },
        new Student(){
            Id = 4,
            FirstName = "A",
            LastName = "B",
            DateOfBirth = new DateOnly(1999, 8, 31),
            Email = "ab@random-tu.de",
            GroupId = 1,
        },
        new Student(){
            Id = 5,
            FirstName = "Fax",
            LastName = "Machine",
            DateOfBirth = new DateOnly(2001, 3, 21),
            Email = "fm@random-tu.de",
            GroupId = 1,
        },
        new Student(){
            Id = 6,
            FirstName = "Black",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1994, 1, 1),
            Email = "bs@random-tu.de",
            GroupId = 37,
        },
        new Student(){
            Id = 7,
            FirstName = "James",
            LastName = "Baxter",
            DateOfBirth = new DateOnly(2003, 4, 12),
            Email = "jb@random-tu.de",
            GroupId = 2,
        },
        ];

    public static IEnumerable<Student> Students
    {
        get => _students.Select(s =>
        {
            s.Group = _groups.FirstOrDefault(g => g.Id == s.GroupId);
            return s;
        });
    }

    public static IEnumerable<Group> Groups
    {
        get => _groups.Select(g =>
        {
            g.Students = _students.Where(s => s.GroupId == g.Id);
            return g;
        });
    }

    public static void AddStudent(Student student)
    {
        _students.Append(student);
    }
}
