using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Interfaces;

namespace Movies.Application.Common.Behaviors;

public class BaseHandler
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly LinkGenerator _generator;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IMapper _mapper;

    public BaseHandler(IUnitOfWork unitOfWork, LinkGenerator generator, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _generator = generator;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    protected object CreatePaging<TEntity>(string searchQuery, IEnumerable<TEntity> items, int numberOfItems, Paging pagingParams)
    {
        var numberOfPages = (int)Math.Ceiling((double)numberOfItems / pagingParams.PageSize);
        var prev = pagingParams.Page > 0
            ? GetUrl(pagingParams.EndpointName, new { name = searchQuery, page = pagingParams.Page - 1, pagingParams.PageSize })
            : null;

        var next = pagingParams.Page < numberOfPages - 1
            ? GetUrl(pagingParams.EndpointName, new { name = searchQuery, page = pagingParams.Page + 1, pagingParams.PageSize })
            : null;

        var first = GetUrl(pagingParams.EndpointName, new { name = searchQuery, page = 0, pagingParams.PageSize });
        var cur = GetUrl(pagingParams.EndpointName, new { name = searchQuery, pagingParams.Page, pagingParams.PageSize });
        var last = GetUrl(pagingParams.EndpointName, new { name = searchQuery, page = numberOfPages - 1, pagingParams.PageSize });
    
        return new
        {
            First = first,
            Prev = prev,
            Next = next,
            Last = last,
            Current = cur,
            NumberOfPages = numberOfPages,
            NumberOfIems = numberOfItems,
            Items = items
        };
    }

    protected TEntityModel CreateLinks<TEntityModel>(string endpointName, object values, TEntityModel entityModel)
    {
        List<Link> links = new List<Link>();
        switch (endpointName)
        {
            case "GetTitle":
            links.Add(new Link() { Href = GetUrl(endpointName, values), Rel = "self", Method = "GET" });
            links.Add(new Link() { Href = GetUrl("BookmarkTitle", values), Rel = "bookmark", Method = "POST" });
            break;
            default:
            // TO DO
            break;
        }
        entityModel.GetType().GetProperty("Links").SetValue(entityModel, links);
        return entityModel;
    }

    protected string GetUrl(string endpointName, object values)
    {
        /*
        var endpointName = _httpContextAccessor
                            .HttpContext
                            .GetEndpoint()
                            .Metadata
                            .GetMetadata<EndpointNameMetadata>()
                            .EndpointName;
        if (_httpContextAccessor.HttpContext == null || endpointName == null) return null;
        */
        var url = _generator.GetUriByName(_httpContextAccessor.HttpContext, endpointName, values);
        return url;
    }
}
