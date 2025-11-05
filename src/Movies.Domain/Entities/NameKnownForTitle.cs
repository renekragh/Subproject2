
namespace Movies.Domain.Entities;

public partial class NameKnownForTitle
{
    public string Knownfortitle { get; set; } = null!;

    public string Nconst { get; set; } = null!;

    public virtual Name NconstNavigation { get; set; } = null!;
}
