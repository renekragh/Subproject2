
namespace Movies.Domain.Entities;

public partial class ImdbUser
{
    public int Userid { get; set; }

    public string? Email { get; set; }

    public string Name { get; set; } = null!;
    
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime Timestamp { get; set; } //= DateTimeOffset.Now.ToUnixTimeSeconds();

    public virtual ICollection<RatingHistory> RatingHistories { get; set; } = new List<RatingHistory>();

    public virtual ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();

    public virtual ICollection<UserBookmarkName> UserBookmarkNames { get; set; } = new List<UserBookmarkName>();

    public virtual ICollection<UserBookmarkTitle> UserBookmarkTitles { get; set; } = new List<UserBookmarkTitle>();
}
