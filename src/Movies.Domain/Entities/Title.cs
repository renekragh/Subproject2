
namespace Movies.Domain.Entities;

public partial class Title
{
    public string Tconst { get; set; } = null!;

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

    public virtual Title? EpisodeNavigation { get; set; }

    public virtual ICollection<Title> InverseEpisodeNavigation { get; set; } = new List<Title>();

    public virtual ICollection<Localized> Localizeds { get; set; } = new List<Localized>();

    public virtual ICollection<Principal> Principals { get; set; } = new List<Principal>();

    public virtual ICollection<RatingHistory> RatingHistories { get; set; } = new List<RatingHistory>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();

    public virtual ICollection<UserBookmarkTitle> UserBookmarkTitles { get; set; } = new List<UserBookmarkTitle>();
}

