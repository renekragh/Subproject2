using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.SearchesHistory.Models;

public class SearchHistoryModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public string Id { get; set; }
    public string SearchQuery { get; set; } 
    public DateTime Timestamp { get; set; }
    public string Url { get; set; }
}
