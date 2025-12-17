using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface ITitlesHandler
{
    object RetrieveTitles(string endpointName, Paging pagingParams);
    object RetrieveTitle(string endpointName, string id);
    object FindTitles(string endpointName, string name, Paging pagingParams);
}
