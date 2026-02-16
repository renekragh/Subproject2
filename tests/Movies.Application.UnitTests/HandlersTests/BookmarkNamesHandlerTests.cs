using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using AutoFixture;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Movies.Application.Common.Behaviors;
using Movies.Application.Common.Interfaces;
using Movies.Application.Features.Bookmarks.Handlers;
using Movies.Application.Features.Bookmarks.Models;
using Movies.Domain.Entities;
using Movies.Domain.Interfaces;
using Xunit.Abstractions;

namespace Movies.Application.UnitTests.HandlersTests;

public class BookmarkNamesHandlerTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IGenericRepository<Idempotency>> _idempotencyRepository;
    private readonly Mock<IGenericRepository<UserBookmarkName>> _userBookmarkNameRepository;
    private readonly Mock<IGenericRepository<ImdbUser>> _imdbUserRepository;
    private readonly Mock<IUsersRepository> _usersRepository;
    private readonly Mock<LinkGenerator> _linkGenerator;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly Fixture _fixture;
    private readonly BookmarkNamesHandler _bookmarkNamesHandler;

    public BookmarkNamesHandlerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _idempotencyRepository = new Mock<IGenericRepository<Idempotency>>(MockBehavior.Strict);
        _userBookmarkNameRepository = new Mock<IGenericRepository<UserBookmarkName>>(MockBehavior.Strict);
        _imdbUserRepository = new Mock<IGenericRepository<ImdbUser>>(MockBehavior.Strict);
        _usersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
        _linkGenerator = new Mock<LinkGenerator>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        _mapper = new Mapper();
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Userid.ToString()));
        _bookmarkNamesHandler = new BookmarkNamesHandler(_unitOfWork.Object, _linkGenerator.Object, _httpContextAccessor.Object, _mapper);
    }

    [Fact]
    public void BookmarkName_WithNote_ReturnsCreatedAtAction()
    {
        // arrange
        var userBookmarkName = _fixture.Create<UserBookmarkName>();
        _idempotencyRepository.Setup(x => x.RetrieveEntity(It.IsAny<Expression<Func<Idempotency, bool>>>()))
                                .Returns(value: null);
        _idempotencyRepository.Setup(x => x.CreateEntity(It.IsAny<Idempotency>()));
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                                .UserBookmarkNames
                                .Add(It.IsAny<UserBookmarkName>()));
        _unitOfWork.Setup(x => x.Save()).Returns(true);
        
        _userBookmarkNameRepository.Setup(x => x.RetrieveEntity(It.IsAny<Expression<Func<UserBookmarkName, bool>>>()))
                                .Returns(userBookmarkName);
        _unitOfWork.Setup(x => x.GetRepository<Idempotency>())
                                .Returns(_idempotencyRepository.Object);
                                
        _unitOfWork.Setup(x => x.GetRepository<UserBookmarkName>())
                                .Returns(_userBookmarkNameRepository.Object);
                                
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);

        // act
        var result = _bookmarkNamesHandler.BookmarkName(id: null, key: new Guid().ToString(), note: null, endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _userBookmarkNameRepository.VerifyAll();
        _idempotencyRepository.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
        _idempotencyRepository.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
    }

        [Fact]
    public void BookmarkName_WithoutNote_ReturnsCreatedAtAction()
    {
        // arrange
        var userBookmarkName = _fixture.Create<UserBookmarkName>();
        userBookmarkName.Note = null;
        _idempotencyRepository.Setup(x => x.RetrieveEntity(It.IsAny<Expression<Func<Idempotency, bool>>>()))
                                .Returns(value: null);
        _idempotencyRepository.Setup(x => x.CreateEntity(It.IsAny<Idempotency>()));
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                                .UserBookmarkNames
                                .Add(It.IsAny<UserBookmarkName>()));
        _unitOfWork.Setup(x => x.Save()).Returns(true);
        
        _userBookmarkNameRepository.Setup(x => x.RetrieveEntity(It.IsAny<Expression<Func<UserBookmarkName, bool>>>()))
                                .Returns(userBookmarkName);
        _unitOfWork.Setup(x => x.GetRepository<Idempotency>())
                                .Returns(_idempotencyRepository.Object);
                                
        _unitOfWork.Setup(x => x.GetRepository<UserBookmarkName>())
                                .Returns(_userBookmarkNameRepository.Object);
                                
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);

        // act
        var result = _bookmarkNamesHandler.BookmarkName(id: null, key: new Guid().ToString(), note: null, endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _userBookmarkNameRepository.VerifyAll();
        _idempotencyRepository.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
        _idempotencyRepository.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void BookmarkName_SecondRequest_Idempotency_ReturnsBadRequest()
    {
        // arrange
        var idempotency = _fixture.Create<Idempotency>();
        var userBookmarkNames = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        UserBookmarkName userBookmarkName = null;
        _idempotencyRepository.Setup(x => x.RetrieveEntity(It.IsAny<Expression<Func<Idempotency, bool>>>()))
                                .Returns(idempotency);
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                                .UserBookmarkNames)
                                .Returns(userBookmarkNames)
                                .Callback(() => userBookmarkName = userBookmarkNames.First());
        _unitOfWork.Setup(x => x.GetRepository<Idempotency>())
                                .Returns(_idempotencyRepository.Object);
                                
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);

        // act
        var result = _bookmarkNamesHandler.BookmarkName(id: userBookmarkNames.First().Nconst, key: new Guid().ToString(), note: null, endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _idempotencyRepository.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _idempotencyRepository.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void BookmarkName_NoValidUserId_ReturnsInternalServerError()
    {
        // arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Name));
        // act
        var result = _bookmarkNamesHandler.BookmarkName(id: null, key: new Guid().ToString(), note: null, endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Value);
        _httpContextAccessor.VerifyAll();
        _httpContextAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNames_NoArgument_Returns5BookmarkedNames()
    {
        // arrange
        var bookmarkedNames = _fixture.CreateMany<UserBookmarkName>(5).ToList();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()).UserBookmarkNames)
                                .Returns(bookmarkedNames);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        Paging paging = _fixture.Create<Paging>();

        // act
        var objectResult = _bookmarkNamesHandler.GetBookmarkedNames(paging.EndpointName, paging);
        var jsonString = JsonSerializer.Serialize(objectResult);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        var itemsProp = rootElement.GetProperty("Items");
        var items = itemsProp.EnumerateArray();
       
        // assert
        Assert.Equal(5, items.Count());
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNames_NoArgument_Returns0BookmarkedNames()
    {
        // arrange
        var bookmarkedNames = _fixture.CreateMany<UserBookmarkName>(0).ToList();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()).UserBookmarkNames)
                                .Returns(bookmarkedNames);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        Paging paging = _fixture.Create<Paging>();

        // act
        var objectResult = _bookmarkNamesHandler.GetBookmarkedNames(paging.EndpointName, paging);
        var jsonString = JsonSerializer.Serialize(objectResult);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        var itemsProp = rootElement.GetProperty("Items");
        var items = itemsProp.EnumerateArray();
       
        // assert
        Assert.Empty(items);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNames_NoValidUserId_ReturnsInternalServerError()
    {
        // arrange
        Paging paging = _fixture.Create<Paging>();
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Name));
        // act
        var result = _bookmarkNamesHandler.GetBookmarkedNames(paging.EndpointName, paging);
       
        // assert        
        Assert.Null(result);
        _httpContextAccessor.VerifyAll();
        _httpContextAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNameById_With_ValidNameId_ReturnsOneBookmarkedName()
    {
        // arrange        
        var userBookmarkNames  = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        UserBookmarkName userBookmarkName = null;
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                        .UserBookmarkNames)
                        .Returns(userBookmarkNames)
                        .Callback(() => userBookmarkName = userBookmarkNames.First());
        _unitOfWork.Setup(x => x.UsersRepository)
                    .Returns(_usersRepository.Object);

        // act
        var result = (BookmarkNameModel) _bookmarkNamesHandler.GetBookmarkedName(endpointName: "GetNameBookmark", nameId: userBookmarkNames.First().Nconst);

        // assert
        Assert.NotNull(result);
        Assert.Equal(userBookmarkName.Nconst, userBookmarkNames.First().Nconst);
        Assert.Equal(userBookmarkName.Note, result.Note);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNameById_With_InValidNameId_ReturnsZeroBookmarkedName()
    {
        // arrange        
        var userBookmarkNames  = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        UserBookmarkName userBookmarkName = null;
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                        .UserBookmarkNames)
                        .Returns(userBookmarkNames)
                        .Callback(() => userBookmarkName = userBookmarkNames.First());
        _unitOfWork.Setup(x => x.UsersRepository)
                    .Returns(_usersRepository.Object);

        // act
        var result = (BookmarkNameModel) _bookmarkNamesHandler.GetBookmarkedName(endpointName: "GetNameBookmark", userBookmarkNames.Last().Nconst);

        // assert
        Assert.NotNull(result);
        Assert.NotEqual(userBookmarkName.Nconst, userBookmarkNames.Last().Nconst);
        Assert.NotEqual(userBookmarkName.Note, userBookmarkNames.Last().Note);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetBookmarkedNameById_NoValidUserId_ReturnsInternalServerError()
    {
        // arrange
        var userBookmarkNames  = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Name));
        // act
        var result = (BookmarkNameModel) _bookmarkNamesHandler.GetBookmarkedName(endpointName: "GetNameBookmark", nameId: userBookmarkNames.First().Nconst);
       
        // assert        
        Assert.Null(result);
        _httpContextAccessor.VerifyAll();
        _httpContextAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public void UpdateBookmarkedName_WithUpdatedNote_ReturnsOkResponse()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        var userBookmarkNames = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        UserBookmarkName updatedBookmarkedName = null;
       
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(It.IsAny<ImdbUser>());
        _imdbUserRepository.Setup(x => x.UpdateEntity(It.IsAny<ImdbUser>()));                     
        _unitOfWork.Setup(x => x.Save())
                    .Returns(true);
        
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                                    .UserBookmarkNames)
                                    .Returns(userBookmarkNames)
                                    .Callback(() => updatedBookmarkedName = userBookmarkNames
                                    .First());
                                        
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        _unitOfWork.Setup(x => x.GetRepository<ImdbUser>())
                                .Returns(_imdbUserRepository.Object);

        // act
        var result = _bookmarkNamesHandler.UpdateBookmarkedName(nameId: userBookmarkNames.First().Nconst, note: "Updated note", endpointName: "GetNameBookmark");
        var jsonString = JsonSerializer.Serialize(result);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        var value = rootElement.GetProperty("Value");
        var item = value.EnumerateObject().FirstOrDefault(x => x.Name == "item").Value;
        var note = item.EnumerateObject().FirstOrDefault(x => x.Name == "Note").Value.ToString();
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated note", note);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _imdbUserRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
        _imdbUserRepository.VerifyNoOtherCalls();
    }

     [Fact]
    public void UpdateBookmarkedName_WithEmptyNote_ReturnsOkResponse()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        var userBookmarkNames = _fixture.CreateMany<UserBookmarkName>(2).ToList();
        UserBookmarkName updatedBookmarkedName = null;
       
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(It.IsAny<ImdbUser>());
        _imdbUserRepository.Setup(x => x.UpdateEntity(It.IsAny<ImdbUser>()));                     
        _unitOfWork.Setup(x => x.Save())
                    .Returns(true);
        
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>())
                                    .UserBookmarkNames)
                                    .Returns(userBookmarkNames)
                                    .Callback(() => updatedBookmarkedName = userBookmarkNames
                                    .First());
                                        
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        _unitOfWork.Setup(x => x.GetRepository<ImdbUser>())
                                .Returns(_imdbUserRepository.Object);

        // act
        var result = _bookmarkNamesHandler.UpdateBookmarkedName(nameId: userBookmarkNames.First().Nconst, note: null, endpointName: "GetNameBookmark");
        var jsonString = JsonSerializer.Serialize(result);
        var jsonDocument = JsonDocument.Parse(jsonString);
        var rootElement = jsonDocument.RootElement;
        var value = rootElement.GetProperty("Value");
        var item = value.EnumerateObject().FirstOrDefault(x => x.Name == "item").Value;
        var note = item.EnumerateObject().FirstOrDefault(x => x.Name == "Note").Value.ToString();
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(string.Empty, note);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _imdbUserRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
        _imdbUserRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteBookmarkedName_WithValidUser_ReturnsOkResponse()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(imdbUser);
        _userBookmarkNameRepository.Setup(x => x.DeleteEntity(It.IsAny<UserBookmarkName>()));                     
        _unitOfWork.Setup(x => x.Save())
                    .Returns(true);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        _unitOfWork.Setup(x => x.GetRepository<UserBookmarkName>())
                                .Returns(_userBookmarkNameRepository.Object);

        // act
        var result = _bookmarkNamesHandler.DeleteBookmarkedName(nameId: imdbUser.UserBookmarkNames.First().Nconst, endpointName: "GetNameBookmark");
  
        // assert        
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _userBookmarkNameRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteBookmarkedName_WithValidUser_ReturnsNotFound()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(imdbUser);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        // act
        var result = _bookmarkNamesHandler.DeleteBookmarkedName(nameId: "1234A", endpointName: "GetNameBookmark");
  
        // assert        
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteBookmarkedName_NoValidUserId_ReturnsInternalServerError()
    {
        // arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Name));
        // act
        var result = _bookmarkNamesHandler.DeleteBookmarkedName(nameId: "1234A", endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Value);
        _httpContextAccessor.VerifyAll();
        _httpContextAccessor.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteAllBookmarkedNames_WithValidUser_ReturnsOkResponse()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(imdbUser);
        _userBookmarkNameRepository.Setup(x => x.DeleteEntities(It.IsAny<List<UserBookmarkName>>()));                     
        _unitOfWork.Setup(x => x.Save())
                    .Returns(true);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        _unitOfWork.Setup(x => x.GetRepository<UserBookmarkName>())
                                .Returns(_userBookmarkNameRepository.Object);

        // act
        var result = _bookmarkNamesHandler.DeleteAllBookmarkedNames(endpointName: "GetNameBookmark");
  
        // assert        
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _userBookmarkNameRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
        _userBookmarkNameRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteAllBookmarkedNames_WithValidUser_ReturnsNotFound()
    {
        // arrange
        var imdbUser = _fixture.Create<ImdbUser>();
        imdbUser.UserBookmarkNames.Clear();
        _usersRepository.Setup(x => x.GetUserWithNameBookmarks(It.IsAny<int>()))
                        .Returns(imdbUser);
        _unitOfWork.Setup(x => x.UsersRepository)
                                .Returns(_usersRepository.Object);
        // act
        var result = _bookmarkNamesHandler.DeleteAllBookmarkedNames(endpointName: "GetNameBookmark");
  
        // assert        
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
        Assert.NotNull(result.Value);
        _unitOfWork.VerifyAll();
        _usersRepository.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
        _usersRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public void DeleteAllBookmarkedNames_NoValidUserId_ReturnsInternalServerError()
    {
        // arrange
        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                                .Returns(new Claim(ClaimTypes.NameIdentifier, _fixture.Create<ImdbUser>().Name));
        // act
        var result = _bookmarkNamesHandler.DeleteAllBookmarkedNames(endpointName: "GetNameBookmark");
       
        // assert        
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        Assert.NotNull(result.Value);
        _httpContextAccessor.VerifyAll();
        _httpContextAccessor.VerifyNoOtherCalls();
    }
}
