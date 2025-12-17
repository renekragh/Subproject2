using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface IBookmarkNamesHandler
{
    ObjectResult BookmarkName(string id, string key, string note, string endpointName);
    object GetBookmarkedNames(string endpointName, Paging pagingParams);
    object GetBookmarkedName(string endpointName, string titleId);
    ObjectResult UpdateBookmarkedName(string nameId, string note, string endpointName);
    ObjectResult DeleteBookmarkedName(string nameId, string endpointName);
    ObjectResult DeleteAllBookmarkedNames(string endpointName);
}
