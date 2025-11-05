
namespace Movies.Domain.Entities;

public partial class Principal
{
    public string Tconst { get; set; } = null!;

    public string Nconst { get; set; } = null!;

    public int Ordering { get; set; }

    public string? Category { get; set; }

    public string? Job { get; set; }

    public string? Characters { get; set; }

    public virtual Name NconstNavigation { get; set; } = null!;

    public virtual Title TconstNavigation { get; set; } = null!;
}
