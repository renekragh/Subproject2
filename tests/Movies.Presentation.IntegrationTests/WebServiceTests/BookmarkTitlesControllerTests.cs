using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Features.Bookmarks.Models;
using Movies.Infrastructure.DataContext;
using Movies.Presentation.IntegrationTests.Fixtures;
using Movies.Presentation.IntegrationTests.Util;
using Xunit.Abstractions;

namespace Movies.Presentation.IntegrationTests.WebServiceTests;

[Collection("Shared collection")]
public class BookmarkTitlesControllerTests : IAsyncLifetime
{

    private readonly CustomWebAppFactory _factory;
    private  HttpClient _httpCLient;
    private const string TCONST_1 = "tt11111111";
    private const string TCONST_2 = "tt00000000";
    private const string USER_ID = "1";
    private readonly ITestOutputHelper _testOutputHelper;

    public BookmarkTitlesControllerTests(CustomWebAppFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _httpCLient = factory.CreateClient();
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetTitleBookmarks_ShouldReturn3Bookmarks()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));
        
        // act
        string requestUri = "api/bookmark-titles";
        var response = await _httpCLient.GetFromJsonAsync<object>(requestUri);
        var jsonString = JsonSerializer.Serialize(response);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        var itemsProp = rootElement.GetProperty("items");
        var items = itemsProp.EnumerateArray();

        // assert
        Assert.NotNull(response);
        Assert.Equal(3, items.Count());
    }

    [Fact]
    public async Task GetTitleBookmarks_WithNoAuth_ShouldReturn401()
    {
        // arrange
        
        // act
        string requestUri = "api/bookmark-titles";
        var response = await _httpCLient.GetAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTitleBookmarkById_WithValidId_ShouldReturnBookmark()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        var response = await _httpCLient.GetFromJsonAsync<BookmarkTitleModel>(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal($"http://localhost/api/bookmark-titles/{TCONST_1}", response.Url);
        Assert.Equal("my title note1", response.Note);
       // Assert.Equal(3, response.Links.Count);
    }

    [Fact]
    public async Task GetTitleBookmarkById_WithInvalidId_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));
        
        // act
        string invalidId = Guid.NewGuid().ToString();
        var response = await _httpCLient.GetAsync($"api/bookmark-titles/{invalidId}");

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTitleBookmarkById_WithNoAuth_ShouldReturn401()
    {
        // arrange
        
        // act
        var response = await _httpCLient.GetAsync($"api/bookmark-titles/{TCONST_1}");

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkTitle_WithNoAuth_ShouldReturn401()
    {
        // arrange        
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);

        // act
        string requestUri = $"api/titles/{TCONST_2}/bookmarks";
        string note = "my title note";
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkTitleWithNote_WithAuth_ShouldReturn201()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/titles/{TCONST_2}/bookmarks";
        string note = "my title note";
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkTitleNoNote_WithAuth_ShouldReturn201()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/titles/{TCONST_2}/bookmarks";
        string note = null;
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkTitle_Idempotency_ShouldReturn400()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/titles/{TCONST_2}/bookmarks";
        string note = null;
        await _httpCLient.PostAsJsonAsync(requestUri, note);
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkTitle_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        string userId = "-1";
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/titles/{TCONST_2}/bookmarks";
        string note = null;
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedTitle_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        string note = "my title note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedTitle_WithNote_ShouldReturn200()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        string note = "my updated title note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedTitle_WithoutNote_ShouldReturn200()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        string note = null;
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedTitle_WithInvalidTitleId_ShouldReturn500()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_2}";
        string note = "my updated title note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedTitle_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        string note = "my updated title note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTitleBookmark_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTitleBookmark_WithValidId_ShouldReturn200()
    {
        // arrange";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTitleBookmark_AllreadyDeleted_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        await _httpCLient.DeleteAsync(requestUri);
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTitleBookmark_WithInvalidId_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string invalidId = "tt44444444";
        string requestUri = $"api/bookmark-titles/{invalidId}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTitleBookmark_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-titles/{TCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllTitleBookmarks_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-titles";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllTitleBookmarks_ShouldReturn200()
    {
        // arrange";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllTitleBookmarks_AllreadyDeleted_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-titles";
        await _httpCLient.DeleteAsync(requestUri);
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllTitleBookmarks_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-titles";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }
}
