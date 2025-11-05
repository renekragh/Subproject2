
namespace Movies.Domain.Entities;

public partial class TitleGenre
{
    public string Genre { get; set; } = null!;

    public string Tconst { get; set; } = null!;

    public virtual Title TconstNavigation { get; set; } = null!;
}
