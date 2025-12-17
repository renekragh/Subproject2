using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using AutoFixture;
using Mapster;
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
using Xunit;

namespace Movies.Application.UnitTests;

public class HandlersTest
{
    protected readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IUsersRepository> _usersRepository;
    private readonly Mock<LinkGenerator> _linkGenerator;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly Fixture _fixture;
    private  readonly BookmarkNamesHandler _bookmarkNamesHandler;

    public HandlersTest()
    {
        _unitOfWork = new Mock<IUnitOfWork>(_usersRepository) {CallBase = true};
        _usersRepository = new Mock<IUsersRepository>();
        _linkGenerator = new Mock<LinkGenerator>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(BookmarkNamesHandler).Assembly);
        _mapper = new Mapper(config);
        _fixture = new Fixture();
        _bookmarkNamesHandler = new BookmarkNamesHandler(_unitOfWork.Object, _linkGenerator.Object, _httpContextAccessor.Object, _mapper);
    }

    [Fact]
    public void GetBookmarkedNames_NoArgument_Returns10BookmarkedNames()
    {
        var bookmarkNameModel = _mapper.Map<BookmarkNameModel>(new UserBookmarkName());
        var bookmarkedNames = _fixture.CreateMany<BookmarkNameModel>(10).ToList();

        _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim(ClaimTypes.NameIdentifier, "33"));

         
            
       // _unitOfWork.Setup(x => x.GetRepository<BookmarkNameModel>().FindEntities(x => true, 0, 0)).Returns(bookmarkedNames);
        _unitOfWork.Setup(x => x.UsersRepository);

       
   
        // act
        var result = (List<BookmarkNameModel>) _bookmarkNamesHandler.GetBookmarkedNames("null", new Paging() {Page = 12, PageSize = 4});

        // assert
        Assert.Equal(10, result.Count);
    }
}
