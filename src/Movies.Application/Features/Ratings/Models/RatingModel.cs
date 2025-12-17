using Movies.Application.Common.Behaviors;

namespace Movies.Application.Features.Ratings.Models;

public class RatingModel
{
    public ICollection<Link> Links { get; set; } = new List<Link>();
    
    public string Url { get; set; }

    public decimal Averagerating { get; set; }

    public int Numvotes { get; set; }
}
