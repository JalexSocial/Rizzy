using System.Text.Json;
using Humanizer;
using RizzyDemo.Controllers.Home.Models;
using RizzyDemo.Helpers;

namespace RizzyDemo;

public class PostRepo
{
	public async Task<List<Post>> GetPosts()
	{
		await Task.Delay(500); 

		var postContent = EmbeddedResourceReader.ReadResourceText("RizzyDemo.Controllers.Home.Models.posts.json");
		var posts = JsonSerializer.Deserialize<List<Post>>(postContent) ?? new();

		Random rand = new Random(System.Environment.TickCount);
		int minutes = 2;
		foreach (var post in posts)
		{
			minutes += rand.Next(2, 120);
			post.TimeOfPost = DateTime.Now.Subtract(new TimeSpan(0, 0, minutes, 0));
			post.RelativeTimeOfPost = post.TimeOfPost.Humanize();
		}

        return posts;
    }

}
