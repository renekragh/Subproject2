using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Titles.Models;
using Movies.Domain.Entities;
using Movies.Domain.ValueObjects.SearchResults;

namespace Movies.Application.Features.Titles.Handlers;

public class TitlesHandler : BaseHandler, ITitlesHandler
{
    private const string GetName = "GetName";
    private const string TvEpisode = "tvEpisode";

    public TitlesHandler(IUnitOfWork unitOfWork, 
                         LinkGenerator generator, 
                         IHttpContextAccessor httpContextAccessor, 
                         IMapper mapper) : base(unitOfWork, generator, httpContextAccessor, mapper) {}


    public object RetrieveTitles(string endpointName, Paging pagingParams)
    {
        var titles = _unitOfWork
                        //.GetRepository<Title>()
                        //.RetrieveEntities(pagingParams.Page, pagingParams.PageSize)
                        .TitlesRepository
                        .GetTitlesWithRating(pagingParams.Page, pagingParams.PageSize)
                        .Select(x => CreateTitleListModel(endpointName, x));

        var numOfItems = _unitOfWork
                            .GetRepository<Title>()
                            .GetEntityCount(predicate: null);

        return CreatePaging(searchQuery: null, titles, numOfItems, pagingParams);
    }
    
    public object RetrieveTitle(string endpointName, string id)
    {
        var title = _unitOfWork
                        .TitlesRepository
                        .GetTitleWithRelatedEntities(id);

        if (title == null) return null;
        var titleModel = CreateTitleModel(endpointName, title);
        return CreateLinks(endpointName, new { id = title.Tconst }, titleModel);
    }

    public object FindTitles(string endpointName, string query, Paging pagingParams)
    {
        Claim nameIdentifier = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        int userId = -1;
        if (nameIdentifier != null) _ = int.TryParse(nameIdentifier.Value, out userId);

//Use stored psql function string_search(...) through EF
        var titles = _unitOfWork
                        .TitlesRepository
                        .GetTitleSearchResults(query, userId).AsEnumerable()
                        .Select(x => CreateTitleSearchResultModel(endpointName, x));

        var numOfItems = _unitOfWork.GetRepository<Title>()
                            .GetEntityCount(x => x.Primarytitle.Contains(query));
                            
        if (numOfItems == 0) return null;    
        if (numOfItems <= pagingParams.PageSize) return new { NumberOfPages = 0, NumberOfIems = numOfItems, Items = titles };                
        return CreatePaging(query, titles, numOfItems, pagingParams);
    }

    private TitleListModel CreateTitleListModel(string endpointName, Title entity)
    {
        TypeAdapterConfig<Title, TitleListModel>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Tconst)
            .Map(dest => dest.Ratings, src => src.Ratings);
           
        var titleListModel = _mapper.Map<TitleListModel>(entity);
        titleListModel.Url = GetUrl(endpointName, new { id = entity.Tconst });
        return titleListModel;
    }

    private TitleModel CreateTitleModel(string endpointName, Title entity)
    {
        TypeAdapterConfig<Title, TitleModel>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Tconst)
            .Map(dest => dest.Genres, src => src.TitleGenres)
            .Map(dest => dest.Localizeds, src => src.Localizeds)
            .Map(dest => dest.Ratings, src => src.Ratings)
            .Map(dest => dest.Principals, src => src.Principals)
            .IgnoreIf((src, dest) => !src.Titletype.Equals(TvEpisode), dest => dest.Episode);
            
        var titleModel = _mapper.Map<TitleModel>(entity); 

        titleModel.Url = GetUrl(endpointName, new { id = entity.Tconst });
        if (entity.Titletype.Equals(TvEpisode)) titleModel.Episode = GetUrl(endpointName, new { id = entity.Tconst });

        foreach (var (index, item) in titleModel.Principals.Index()) 
        {
            item.Url = GetUrl(GetName, new { id = entity.Principals.Select(x => x.Nconst).ElementAt(index)});
        }
        return titleModel;
    }

        private TitleSearchResultModel CreateTitleSearchResultModel(string endpointName, TitleSearchResults searchResults)
    {
        TypeAdapterConfig<TitleSearchResults, TitleSearchResultModel>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Tconst);
          
        var titleSearchResultModel = _mapper.Map<TitleSearchResultModel>(searchResults);
        titleSearchResultModel.Url = GetUrl(endpointName, new { id = searchResults.Tconst });
        return titleSearchResultModel;
    }
}