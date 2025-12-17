using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Names.Models;
using Movies.Domain.Entities;

namespace Movies.Application.Features.Names.Handlers;

public class NamesHandler : BaseHandler, INamesHandler
{
    public NamesHandler(IUnitOfWork unitOfWork, 
                         LinkGenerator generator, 
                         IHttpContextAccessor httpContextAccessor, 
                         IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}

    public object GetNames(string endpointName, Paging pagingParams)
    {
        var titles = _unitOfWork
                        .GetRepository<Name>()
                        .RetrieveEntities(pagingParams.Page, pagingParams.PageSize)
                        .Select(x => CreateNameListModel(endpointName, x));
                            
        var numOfItems = _unitOfWork
                            .GetRepository<Name>()
                            .GetEntityCount(predicate: null);

        return CreatePaging(searchQuery: null, titles, numOfItems, pagingParams);
    }

    public object GetName(string endpointName, string id)
    {
        var name = _unitOfWork
                    .NamesRepository
                    .GetNameWithRelatedEntities(id);

        if (name == null) return null;
        var nameModel = CreateNameModel(endpointName, name);
        return CreateLinks(endpointName, new { id = name.Nconst }, nameModel);
    }

    public object FindNames(string endpointName, string query, Paging pagingParams)
    {
        var titles = _unitOfWork
                        .GetRepository<Name>()
                        .FindEntities(x => x.Primaryname.Contains(query), pagingParams.Page, pagingParams.PageSize)
                        .Select(x => CreateNameListModel(endpointName, x));
        
        var numOfItems = _unitOfWork.GetRepository<Name>()
                            .GetEntityCount(x => x.Primaryname.Contains(query));
                            
        if (numOfItems == 0) return null;    
        if (numOfItems <= pagingParams.PageSize) return new { NumberOfPages = 0, NumberOfIems = numOfItems, Items = titles };                
        return CreatePaging(query, titles, numOfItems, pagingParams);
    }

    private NameListModel CreateNameListModel(string endpointName, Name entity)
    {
        var nameListModel = _mapper.Map<NameListModel>(entity);
        nameListModel.Url = GetUrl(endpointName, new { id = entity.Nconst });
        return nameListModel;
    }

    private NameModel CreateNameModel(string endpointName, Name entity)
    {
        TypeAdapterConfig<Name, NameModel>
            .NewConfig()
            .Map(dest => dest.NameKnownForTitles, src => src.NameKnownForTitles)
            .Map(dest => dest.NamePrimaryProfessions, src => src.NamePrimaryProfessions)
            .Map(dest => dest.Principals, src => src.Principals)
            .Map(dest => dest.UserBookmarkNames, src => src.UserBookmarkNames);
            
        var nameModel = _mapper.Map<NameModel>(entity); 

        nameModel.Url = GetUrl(endpointName, new { id = entity.Nconst });
    
        foreach (var (index, item) in nameModel.Principals.Index()) 
        {
            item.Url = GetUrl(nameof(GetName), new { id = entity.Principals.Select(x => x.Nconst).ElementAt(index)});
        }
        return nameModel;
    }
}
