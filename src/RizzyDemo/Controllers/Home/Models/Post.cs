namespace RizzyDemo.Controllers.Home.Models;

public class Post
{
    public string Id { get; set; } = string.Empty;
    public string ProfilePhotoUrl { get; set; } = string.Empty;
    public string MediaPhotoUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime TimeOfPost { get; set; } = DateTime.Now;
    public string RelativeTimeOfPost { get; set; } = string.Empty;
    public int Likes { get; set; }
}
