using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[ApiController]
[Authorize]
public class BookmarkNamesController : ControllerBase
{
    private readonly IBookmarkNamesHandler _bookmarkNamesHandler;

    public BookmarkNamesController(IBookmarkNamesHandler bookmarkNamesHandler)
    {
        _bookmarkNamesHandler = bookmarkNamesHandler;
    }

    [HttpPost("api/names/{id}/bookmarks", Name = nameof(BookmarkName))] 
    public IActionResult BookmarkName(string id, [FromHeader(Name = "Idempotency-Key")] string key, [FromBody] string? note)
    {
        return _bookmarkNamesHandler.BookmarkName(id, key, note, nameof(GetNameBookmark));
    }

    [HttpGet("api/bookmark-names", Name = nameof(GetNameBookmarks))]
    public IActionResult GetNameBookmarks([FromQuery] Paging pagingParams)
    {
        var bookmarkNameModel = _bookmarkNamesHandler.GetBookmarkedNames(nameof(GetNameBookmark), pagingParams);
        if (bookmarkNameModel == null) return NotFound();
        return Ok(bookmarkNameModel);
    }

    [HttpGet("api/bookmark-names/{id}", Name = nameof(GetNameBookmark))]
    public IActionResult GetNameBookmark(string id)
    {
        var bookmarkNameModel = _bookmarkNamesHandler.GetBookmarkedName(nameof(GetNameBookmark), id);
        if (bookmarkNameModel == null) return NotFound();
        return Ok(bookmarkNameModel);
    }

    [HttpPut("api/bookmark-names/{id}", Name = nameof(UpdateNameBookmark))]
    public IActionResult UpdateNameBookmark(string id, [FromBody] string? note)
    {
        var bookmarkNameModel = _bookmarkNamesHandler.UpdateBookmarkedName(id, note, nameof(GetNameBookmark));
        if (bookmarkNameModel == null) return NotFound();
        return Ok(bookmarkNameModel);
    }

    [HttpDelete("api/bookmark-names/{id}", Name = nameof(DeleteNameBookmark))] 
    public IActionResult DeleteNameBookmark(string id)
    {
        return _bookmarkNamesHandler.DeleteBookmarkedName(id, nameof(GetNameBookmark));
    }

    [HttpDelete("api/bookmark-names", Name = nameof(DeleteAllNameBookmarks))] 
    public IActionResult DeleteAllNameBookmarks()
    {
        return _bookmarkNamesHandler.DeleteAllBookmarkedNames(nameof(GetNameBookmark));
    }
}

