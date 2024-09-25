namespace RizzyDemo.Controllers.Home.Models;

public record Author(string Username, string Image);
public record Article(string Title, string Body, string Description, DateTime UpdatedAt, Author Author);
