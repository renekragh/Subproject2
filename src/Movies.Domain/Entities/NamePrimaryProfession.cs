
namespace Movies.Domain.Entities;

public partial class NamePrimaryProfession
{
    public string Primaryprofession { get; set; } = null!;

    public string Nconst { get; set; } = null!;

    public virtual Name NconstNavigation { get; set; } = null!;
}
