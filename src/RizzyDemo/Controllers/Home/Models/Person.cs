using System.ComponentModel.DataAnnotations;

namespace RizzyDemo.Controllers.Home.Models;

public class Person
{
	public class UserAddress
	{
		[Required]
		public string City { get; set; } = string.Empty;
	}

    [Required]
    [StringLength(10)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public UserAddress Address { get; set; } = new();
}
