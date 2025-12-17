using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Names.Models;

public class NameModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();

    public string Url { get; set; }
    
    public string Primaryname { get; set; }

    public string Birthyear { get; set; }

    public string Deathyear { get; set; }

    public virtual ICollection<KnownForTitleListModel> NameKnownForTitles { get; set; } = new List<KnownForTitleListModel>();

    public virtual ICollection<PrimaryProfessionListModel> NamePrimaryProfessions { get; set; } = new List<PrimaryProfessionListModel>();

    public virtual ICollection<PrincipalListModel> Principals { get; set; } = new List<PrincipalListModel>();

    public virtual ICollection<BookmarkNameListModel> UserBookmarkNames { get; set; } = new List<BookmarkNameListModel>();
}
