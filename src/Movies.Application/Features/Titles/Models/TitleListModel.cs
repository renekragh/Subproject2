using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Titles.Models;

public class TitleListModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Id { get; set; }

    public string Url { get; set; }

    public string Titletype { get; set; }

    public string Primarytitle { get; set; }

    public ICollection<RatingListModel> Ratings { get; set; } = new List<RatingListModel>();
}
