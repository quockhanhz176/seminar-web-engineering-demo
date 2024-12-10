
using System.ComponentModel.DataAnnotations;

namespace demo_project.Models;

public class Group
{
    [Key]
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public IEnumerable<Student> Students { get; set; }
}
