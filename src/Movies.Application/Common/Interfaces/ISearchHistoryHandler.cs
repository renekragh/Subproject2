using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface ISearchHistoryHandler
{
    object GetAllSearchHistory(string endpointName, Paging pagingParams);
    object GetSearchHistory(string endpointName, int id);
    object DeleteSearchHistory(string endpointName, int id);
    object DeleteAllSearchHistory(string endpointName);
}
