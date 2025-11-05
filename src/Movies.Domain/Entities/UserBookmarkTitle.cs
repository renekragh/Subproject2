
namespace Movies.Domain.Entities;

public partial class UserBookmarkTitle
{
    public string Tconst { get; set; } = null!;

    public int Userid { get; set; }

    public string? Note { get; set; }

    public virtual Title TconstNavigation { get; set; } = null!;

    public virtual ImdbUser User { get; set; } = null!;
}
