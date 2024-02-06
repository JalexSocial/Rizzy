using System.ComponentModel.DataAnnotations;

namespace RizzyDemo.Controllers.Home.Models;

public class Person
{
    [Required]
    [StringLength(10)]
    public string Name { get; set; } = string.Empty;
}
