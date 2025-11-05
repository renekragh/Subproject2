
namespace Movies.Domain.Entities;

public partial class Localized
{
    public string Titleid { get; set; } = null!;

    public int Ordering { get; set; }

    public string? Title { get; set; }

    public string? Region { get; set; }

    public string? Language { get; set; }

    public string? Types { get; set; }

    public string? Attributes { get; set; }

    public bool? Isoriginaltitle { get; set; }

    public string? Tconst { get; set; }

    public virtual Title? TconstNavigation { get; set; }
}
