using System.ComponentModel.DataAnnotations;

namespace demo_project.Models;

public class Student
{
    [Key]
    public uint Id { get; set; }

    public string? FirstName { get; set; } = null!;

    public string? LastName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Email { get; set; } = null!;

    public uint? GroupId { get; set; }

    public Group? Group { get; set; }
}
