
namespace Movies.Domain.Entities;

public partial class Rating
{
    public int Ratingid { get; set; }

    public decimal? Averagerating { get; set; }

    public int? Numvotes { get; set; }

    public string? Tconst { get; set; }

    public virtual Title? TconstNavigation { get; set; }
}
