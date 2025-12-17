using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Bookmarks.Models;

public class BookmarkNameModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Url { get; set; }

    public string Note { get; set; }
}
