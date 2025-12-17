using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.RatingsHistory.Models;

public class RatingHistoryModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public string Url { get; set; }
    public short Rating { get; set; }
}
