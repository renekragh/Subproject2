
namespace Movies.Domain.Entities;

public partial class UserBookmarkName
{
    public string Nconst { get; set; } = null!;

    public int Userid { get; set; }

    public string? Note { get; set; }

    public virtual Name NconstNavigation { get; set; } = null!;

    public virtual ImdbUser User { get; set; } = null!;
}
