
namespace Movies.Domain.Entities;

public partial class Name
{
    public string Nconst { get; set; } = null!;

    public string? Primaryname { get; set; }

    public string? Birthyear { get; set; }

    public string? Deathyear { get; set; }

    public virtual ICollection<NameKnownForTitle> NameKnownForTitles { get; set; } = new List<NameKnownForTitle>();

    public virtual ICollection<NamePrimaryProfession> NamePrimaryProfessions { get; set; } = new List<NamePrimaryProfession>();

    public virtual ICollection<Principal> Principals { get; set; } = new List<Principal>();

    public virtual ICollection<UserBookmarkName> UserBookmarkNames { get; set; } = new List<UserBookmarkName>();
}
