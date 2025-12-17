using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Titles.Models;

public class TitleListModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Url { get; set; }

    public string Titletype { get; set; }

    public string Primarytitle { get; set; }
}
