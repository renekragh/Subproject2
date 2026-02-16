using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Bookmarks.Models;

public class BookmarkTitleModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();

    public string Id { get; set; }
    
    public string Url { get; set; }

    public string Note { get; set; }
}
