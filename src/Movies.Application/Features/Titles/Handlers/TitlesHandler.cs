using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Titles.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Titles.Handlers;

public class TitlesHandler : ITitlesHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LinkGenerator _generator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public TitlesHandler(IUnitOfWork unitOfWork, LinkGenerator generator, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _generator = generator;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public object RetrieveTitles(int page, int pageSize)
    {
        var titles = _unitOfWork.GetRepository<Title>()
                            .RetrieveEntities(page, pageSize)
                            .Select(x => CreateTitleListModel(x));
                            
        // .OrderBy(x => x.Primarytitle);

        var numOfItems = _unitOfWork.GetRepository<Title>()
                            .GetEntityCount();

        return CreatePaging(titles, numOfItems, page, pageSize);
    }
    
    public TitleModel? RetrieveTitle(string id)
        {
            var title = _unitOfWork.GetRepository<Title>().FindEntity(id);
            if (title != null) return CreateTitleModel(title);
            return null;
        }

    public object FindTitles(string name, int page, int pageSize)
    {
        var titles = _unitOfWork.GetRepository<Title>()
                            .FindEntities(x => x.Primarytitle
                            !.Contains(name), page, pageSize)
                            .Select(CreateTitleListModel);
        //.OrderBy(x => x.Primarytitle);

        var numOfItems = _unitOfWork.GetRepository<Title>()
                            .GetEntityCount();
                            
        return CreatePaging(titles, numOfItems, page, pageSize);
    }

    private TitleModel CreateTitleModel(Title title)
    {
        var titleModel = _mapper.Map<TitleModel>(title);
        titleModel.Url = GetUrl(new { id = title.Tconst });
        return titleModel;
    }

    private TitleListModel CreateTitleListModel(Title title)
    {
        var titleListModel = _mapper.Map<TitleListModel>(title);
        titleListModel.Url = GetUrl(new { tconst = title.Tconst });
        return titleListModel;
    }

    private object CreatePaging<TEntity>(IEnumerable<TEntity> items, int numberOfItems, int page, int pageSize)
    {
        var numberOfPages = (int)Math.Ceiling((double)numberOfItems / pageSize);

        var prev = page > 0
            ? GetUrl(new { page = page - 1, pageSize })
            : null;

        var next = page < numberOfPages - 1
            ? GetUrl(new { page = page + 1, pageSize })
            : null;

        var first = GetUrl(new { page = 0, pageSize });
        var cur = GetUrl(new { page, pageSize });
        var last = GetUrl(new { page = numberOfPages - 1, pageSize });

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
 
    private string? GetUrl(object values)
    {
        var endpointName = _httpContextAccessor
                            ?.HttpContext
                            ?.GetEndpoint()
                            ?.Metadata
                            ?.GetMetadata<EndpointNameMetadata>()
                            ?.EndpointName;
        if (_httpContextAccessor?.HttpContext == null || endpointName == null) return null;
        var url = _generator.GetUriByName(_httpContextAccessor.HttpContext, endpointName, values);
        return url;
    }
}