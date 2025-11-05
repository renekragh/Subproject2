namespace Movies.Application.Features.Titles.Models;

public class TitleModel
{
    public string? Url { get; set; }

    public string? Titletype { get; set; }

    public string? Primarytitle { get; set; }

    public string? Originaltitle { get; set; }

    public bool? Isadult { get; set; }

    public string? Startyear { get; set; }

    public string? Endyear { get; set; }

    public int? Runtimeinminutes { get; set; }

    public string? Poster { get; set; }

    public string? Plot { get; set; }

    public int? Seasonnumber { get; set; }

    public int? Episodenumber { get; set; }

    public string? Episode { get; set; }
}
