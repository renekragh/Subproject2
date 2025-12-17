namespace Movies.Application.Common.Behaviors;

public class Paging
{
    public string EndpointName { get; set; }
    private const int MaxPageSize = 25;
    public int PageSize { get; set; } = 10;
    private int page = 0;

    public int Page {
        get { return page; }
        set { page = value > MaxPageSize ? MaxPageSize : value; } 
    }

}
