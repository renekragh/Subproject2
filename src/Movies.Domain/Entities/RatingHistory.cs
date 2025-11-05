
namespace Movies.Domain.Entities;

public partial class RatingHistory
{
    public string Tconst { get; set; } = null!;

    public int Userid { get; set; }

    public short Rating { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Title TconstNavigation { get; set; } = null!;

    public virtual ImdbUser User { get; set; } = null!;
}
