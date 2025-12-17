using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;

namespace Movies.WebApi.Controllers;

[ApiController]
[Authorize]
public class BookmarkTitlesController : ControllerBase
{
    private readonly IBookmarkTitlesHandler _bookmarkTitlesHandler;

    public BookmarkTitlesController(IBookmarkTitlesHandler bookmarkTitlesHandler)
    {
        _bookmarkTitlesHandler = bookmarkTitlesHandler;
    }

    [HttpPost("api/titles/{id}/bookmarks", Name = nameof(BookmarkTitle))] 
    public IActionResult BookmarkTitle(string id, [FromHeader(Name = "Idempotency-Key")] string key, [FromBody] string? note)
    {
        return _bookmarkTitlesHandler.BookmarkTitle(id, key, note, nameof(GetTitleBookmark));
    }

    [HttpGet("api/bookmark-titles", Name = nameof(GetTitleBookmarks))]
    public IActionResult GetTitleBookmarks([FromQuery] Paging pagingParams)
    {
        var bookmarkTitleModel = _bookmarkTitlesHandler.GetBookmarkedTitles(nameof(GetTitleBookmark), pagingParams);
        if (bookmarkTitleModel == null) return NotFound();
        return Ok(bookmarkTitleModel);
    }

    [HttpGet("api/bookmark-titles/{id}", Name = nameof(GetTitleBookmark))]
    public IActionResult GetTitleBookmark(string id)
    {
        var bookmarkTitleModel = _bookmarkTitlesHandler.GetBookmarkedTitle(nameof(GetTitleBookmark), id);
        if (bookmarkTitleModel == null) return NotFound();
        return Ok(bookmarkTitleModel);
    }

    [HttpPut("api/bookmark-titles/{id}", Name = nameof(UpdateTitleBookmark))]
    public IActionResult UpdateTitleBookmark(string id, [FromBody] string? note)
    {
        var bookmarkTitleModel = _bookmarkTitlesHandler.UpdateBookmarkedTitle(id, note, nameof(GetTitleBookmark));
        if (bookmarkTitleModel == null) return NotFound();
        return Ok(bookmarkTitleModel);
    }

    [HttpDelete("api/bookmark-titles/{id}", Name = nameof(DeleteTitleBookmark))] 
    public IActionResult DeleteTitleBookmark(string id)
    {
        return _bookmarkTitlesHandler.DeleteBookmarkedTitle(id, nameof(GetTitleBookmark));
    }

    [HttpDelete("api/bookmark-titles", Name = nameof(DeleteAllTitleBookmarks))] 
    public IActionResult DeleteAllTitleBookmarks()
    {
        return _bookmarkTitlesHandler.DeleteAllBookmarkedTitles(nameof(GetTitleBookmark));
    }
}