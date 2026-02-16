using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Titles.Models;

public class TitleModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Id { get; set; } 
    
    public string Url { get; set; }

    public string Titletype { get; set; }

    public string Primarytitle { get; set; }

    public string Originaltitle { get; set; }

    public bool Isadult { get; set; }

    public string Startyear { get; set; }

    public string Endyear { get; set; }

    public int Runtimeinminutes { get; set; }

    public string Poster { get; set; }

    public string Plot { get; set; }

    public int Seasonnumber { get; set; }

    public int Episodenumber { get; set; }

    public string Episode { get; set; }

    public ICollection<GenreListModel> Genres { get; set; } = new List<GenreListModel>();

    public ICollection<LocalizedListModel> Localizeds { get; set; } = new List<LocalizedListModel>();

    public ICollection<RatingListModel> Ratings { get; set; } = new List<RatingListModel>();
    
    public ICollection<PrincipalListModel> Principals { get; set; } = new List<PrincipalListModel>();
}
