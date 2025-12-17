using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Titles.Models;

public class TitleSearchResultModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Url { get; set; }

    public string Primarytitle { get; set; }
}
