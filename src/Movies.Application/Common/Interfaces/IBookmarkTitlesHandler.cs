using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface IBookmarkTitlesHandler
{
    ObjectResult BookmarkTitle(string id, string key, string note, string endpointName);
    object GetBookmarkedTitles(string endpointName, Paging pagingParams);
    object GetBookmarkedTitle(string endpointName, string titleId);
    ObjectResult UpdateBookmarkedTitle(string titleId, string note, string endpointName);
    ObjectResult DeleteBookmarkedTitle(string titleId, string endpointName);
    ObjectResult DeleteAllBookmarkedTitles(string endpointName);
}
