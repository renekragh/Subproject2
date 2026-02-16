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
public class BookmarkNamesControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private  HttpClient _httpCLient;
    private const string NCONST_1 = "nm11111111";
    private const string NCONST_2 = "nm00000000";
    private const string USER_ID = "1";
    private readonly ITestOutputHelper _testOutputHelper;

    public BookmarkNamesControllerTests(CustomWebAppFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _httpCLient = factory.CreateClient();
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetNameBookmarks_ShouldReturn3Bookmarks()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));
        
        // act
        string requestUri = "api/bookmark-names";
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
    public async Task GetNameBookmarks_WithNoAuth_ShouldReturn401()
    {
        // arrange
        
        // act
        string requestUri = "api/bookmark-names";
        var response = await _httpCLient.GetAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetNameBookmarkById_WithValidId_ShouldReturnBookmark()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        var response = await _httpCLient.GetFromJsonAsync<BookmarkNameModel>(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal($"http://localhost/api/bookmark-names/{NCONST_1}", response.Url);
        Assert.Equal("my name note1", response.Note);
       // Assert.Equal(3, response.Links.Count);
    }

    [Fact]
    public async Task GetNameBookmarkById_WithInvalidId_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));
        
        // act
        string invalidId = Guid.NewGuid().ToString();
        var response = await _httpCLient.GetAsync($"api/bookmark-names/{invalidId}");

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNameBookmarkById_WithNoAuth_ShouldReturn401()
    {
        // arrange
        
        // act
        var response = await _httpCLient.GetAsync($"api/bookmark-names/{NCONST_1}");

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkName_WithNoAuth_ShouldReturn401()
    {
        // arrange        
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);

        // act
        string requestUri = $"api/names/{NCONST_2}/bookmarks";
        string note = "my name note";
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkNameWithNote_WithAuth_ShouldReturn201()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/names/{NCONST_2}/bookmarks";
        string note = "my name note";
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkNameNoNote_WithAuth_ShouldReturn201()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/names/{NCONST_2}/bookmarks";
        string note = null;
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkName_Idempotency_ShouldReturn400()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/names/{NCONST_2}/bookmarks";
        string note = null;
        await _httpCLient.PostAsJsonAsync(requestUri, note);
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkName_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string key = Guid.NewGuid().ToString();
        string userId = "-1";
        _httpCLient.DefaultRequestHeaders.Add("Idempotency-Key", key);
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/names/{NCONST_2}/bookmarks";
        string note = null;
        var response = await _httpCLient.PostAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedName_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        string note = "my name note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedName_WithNote_ShouldReturn200()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        string note = "my updated name note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedName_WithoutNote_ShouldReturn200()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        string note = null;
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedName_WithInvalidNameId_ShouldReturn500()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_2}";
        string note = "my updated name note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookmarkedName_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        string note = "my updated name note";
        var response = await _httpCLient.PutAsJsonAsync(requestUri, note);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }


    [Fact]
    public async Task DeleteNameBookmark_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNameBookmark_WithValidId_ShouldReturn200()
    {
        // arrange";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNameBookmark_AllreadyDeleted_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        await _httpCLient.DeleteAsync(requestUri);
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNameBookmark_WithInvalidId_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string invalidId = "nm44444444";
        string requestUri = $"api/bookmark-names/{invalidId}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNameBookmark_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-names/{NCONST_1}";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllNameBookmarks_WithNoAuth_ShouldReturn401()
    {
        // arrange

        // act
        string requestUri = $"api/bookmark-names";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllNameBookmarks_ShouldReturn200()
    {
        // arrange";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names";
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllNameBookmarks_AllreadyDeleted_ShouldReturn404()
    {
        // arrange
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(USER_ID));

        // act
        string requestUri = $"api/bookmark-names";
        await _httpCLient.DeleteAsync(requestUri);
        var response = await _httpCLient.DeleteAsync(requestUri);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllNameBookmarks_WithInvalidUserId_ShouldReturn500()
    {
        // arrange
        string userId = "-1";
        _httpCLient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(userId));

        // act
        string requestUri = $"api/bookmark-names";
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
