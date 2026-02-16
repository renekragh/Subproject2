using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Names.Models;

public class NameListModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public string Id { get; set; }
    public string Url { get; set; }
    public string Primaryname { get; set; }
}
