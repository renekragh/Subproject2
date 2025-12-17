using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.SearchesHistory.Models;

public class SearchHistoryModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public string Url { get; set; }
}
