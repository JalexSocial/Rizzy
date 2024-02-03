using System.ComponentModel.DataAnnotations;

namespace RizzyDemo.Controllers.Home.Models;

public class Person
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
