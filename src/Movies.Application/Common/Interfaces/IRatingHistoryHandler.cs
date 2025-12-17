using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface IRatingHistoryHandler
{
    object RetrieveRatingHistories(string endpointName, Paging pagingParams);
    object RetrieveRatingHistory(string endpointName, string id);
}
