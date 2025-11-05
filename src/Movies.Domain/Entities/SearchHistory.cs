
namespace Movies.Domain.Entities;

public partial class SearchHistory
{
    public int Searchid { get; set; }

    public string SearchQuery { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int? Userid { get; set; }

    public virtual ImdbUser? User { get; set; }
}
