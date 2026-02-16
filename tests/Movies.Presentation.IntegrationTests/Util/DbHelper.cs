using Movies.Domain.Entities;
using Movies.Infrastructure.DataContext;

namespace Movies.Presentation.IntegrationTests.Util;

public static class DbHelper
{
    public static void InitDbForTests(PostgresDbContext db)
    {
        db.Names.AddRange(GetNamesForTest());
        db.Titles.AddRange(GetTitlesForTest());
        db.ImdbUsers.AddRange(GetUsersForTest());
        db.SaveChanges();
    }

    public static void ReinitDbForTests(PostgresDbContext db)
    {
        db.UserBookmarkNames.RemoveRange(db.UserBookmarkNames);
        db.UserBookmarkTitles.RemoveRange(db.UserBookmarkTitles);
        db.Names.RemoveRange(db.Names);
        db.Titles.RemoveRange(db.Titles);
        db.ImdbUsers.RemoveRange(db.ImdbUsers);
        db.SaveChanges();
        InitDbForTests(db);
    }

    private static List<Name> GetNamesForTest()
    {
        Name name0 = new Name() {Nconst="nm00000000", Primaryname="Dummy name0"};
        Name name1 = new Name() {Nconst="nm11111111", Primaryname="Dummy name1"};
        Name name2 = new Name() {Nconst="nm22222222", Primaryname="Dummy name2"};
        Name name3 = new Name() {Nconst="nm33333333", Primaryname="Dummy name3"};
        Name name4 = new Name() {Nconst="nm44444444", Primaryname="Dummy name4"};
        Name name5 = new Name() {Nconst="nm55555555", Primaryname="Dummy name5"};
        Name name6 = new Name() {Nconst="nm66666666", Primaryname="Dummy name6"};
        Name name7 = new Name() {Nconst="nm77777777", Primaryname="Dummy name7"};
        List<Name> names = new(4) {name0, name1, name2, name3, name4, name5, name6, name7};
        return names;
    }

    private static List<Title> GetTitlesForTest()
    {
        Title title0 = new Title() {Tconst="tt00000000", Primarytitle="Dummy title0"};
        Title title1 = new Title() {Tconst="tt11111111", Primarytitle="Dummy title1"};
        Title title2 = new Title() {Tconst="tt22222222", Primarytitle="Dummy title2"};
        Title title3 = new Title() {Tconst="tt33333333", Primarytitle="Dummy title3"};
        Title title4 = new Title() {Tconst="tt44444444", Primarytitle="Dummy title4"};
        Title title5 = new Title() {Tconst="tt55555555", Primarytitle="Dummy title5"};
        Title title6 = new Title() {Tconst="tt66666666", Primarytitle="Dummy title6"};
        Title title7 = new Title() {Tconst="tt77777777", Primarytitle="Dummy title7"};
        List<Title> titles = new(4) {title0, title1, title2, title3, title4, title5, title6, title7};
        return titles;
    }

    private static List<ImdbUser> GetUsersForTest()
    {
        ImdbUser imdbUser1 = new ImdbUser() {
            Userid=1, 
            Email="user@one.com", 
            Name="User One", 
            Password="A12B6FF865384B067C47E76104628B2E1EF6DE8B1AF9778F6526F26A129E9ACC",
            UserName="user1",
            Salt="0752A58AF211BB10",
            Role="User"
        };

        ImdbUser imdbUser2 = new ImdbUser() {
            Userid=2, 
            Email="user@two.com", 
            Name="User Two", 
            Password="B12B6FF865384B067C47E76104628F2E1EF6DE8B1AF9778F6526F22A129E9ADE",
            UserName="user2",
            Salt="1752A58BF211BB11",
            Role="User"
        };

        ImdbUser imdbUser3 = new ImdbUser() {
            Userid=3, 
            Email="user@three.com", 
            Name="User Three", 
            Password="B12B6FF965384B063A47E76104628F2E1EF6DE8B2AF9378F6526F22A129E9ADF",
            UserName="user3",
            Salt="1722A58FF211CB15",
            Role="User"
        };

        imdbUser1.UserBookmarkNames = GetNameBookmarksForUser1ForTest();
        imdbUser2.UserBookmarkNames = GetNameBookmarksForUser2ForTest();
        imdbUser3.UserBookmarkNames = GetNameBookmarksForUser3ForTest();
        imdbUser1.UserBookmarkTitles = GetTitleBookmarksForUser1ForTest();
        imdbUser2.UserBookmarkTitles = GetTitleBookmarksForUser2ForTest();
        imdbUser3.UserBookmarkTitles = GetTitleBookmarksForUser3ForTest();
        List<ImdbUser> imdbUsers = new(3) {imdbUser1, imdbUser2, imdbUser3};
        return imdbUsers;
    }

    private static List<UserBookmarkName> GetNameBookmarksForUser1ForTest()
    {
        UserBookmarkName userBookmarkName1 = new UserBookmarkName() {Nconst="nm11111111", Note="my name note1", Userid=1};
        UserBookmarkName userBookmarkName2 = new UserBookmarkName() {Nconst="nm22222222", Note=null, Userid=1};
        UserBookmarkName userBookmarkName3 = new UserBookmarkName() {Nconst="nm33333333", Note="my name bookmark note1", Userid=1};
        List<UserBookmarkName> userBookmarkNames = new(3) {userBookmarkName1, userBookmarkName2, userBookmarkName3};
        return userBookmarkNames;
    }

    private static List<UserBookmarkTitle> GetTitleBookmarksForUser1ForTest()
    {
        UserBookmarkTitle userBookmarkTitle1 = new UserBookmarkTitle() {Tconst="tt11111111", Note="my title note1", Userid=1};
        UserBookmarkTitle  userBookmarkTitle2 = new UserBookmarkTitle() {Tconst="tt22222222", Note=null, Userid=1};
        UserBookmarkTitle  userBookmarkTitle3 = new UserBookmarkTitle() {Tconst="tt33333333", Note="my title bookmark note1", Userid=1};
        List<UserBookmarkTitle > userBookmarkTitles = new(3) {userBookmarkTitle1, userBookmarkTitle2, userBookmarkTitle3};
        return userBookmarkTitles;
    }

    private static List<UserBookmarkName> GetNameBookmarksForUser2ForTest()
    {
        UserBookmarkName userBookmarkName4 = new UserBookmarkName() {Nconst="nm11111111", Note=null, Userid=2};
        UserBookmarkName userBookmarkName5 = new UserBookmarkName() {Nconst="nm44444444", Note="my name note2", Userid=2};
        UserBookmarkName userBookmarkName6 = new UserBookmarkName() {Nconst="nm55555555", Note="my name bookmark note2", Userid=2};
        List<UserBookmarkName> userBookmarkNames = new(3) {userBookmarkName4, userBookmarkName5, userBookmarkName6};
        return userBookmarkNames;
    }
    private static List<UserBookmarkTitle> GetTitleBookmarksForUser2ForTest()
    {
        UserBookmarkTitle userBookmarkTitle4 = new UserBookmarkTitle() {Tconst="tt11111111", Note=null, Userid=2};
        UserBookmarkTitle  userBookmarkTitle5 = new UserBookmarkTitle() {Tconst="tt44444444", Note="my title note2", Userid=2};
        UserBookmarkTitle  userBookmarkTitle6 = new UserBookmarkTitle() {Tconst="tt55555555", Note="my title bookmark note2", Userid=2};
        List<UserBookmarkTitle > userBookmarkTitles = new(3) {userBookmarkTitle4, userBookmarkTitle5, userBookmarkTitle6};
        return userBookmarkTitles;
    }

    private static List<UserBookmarkName> GetNameBookmarksForUser3ForTest()
    {
        UserBookmarkName userBookmarkName7 = new UserBookmarkName() {Nconst="nm22222222", Note="my name note3", Userid=3};
        List<UserBookmarkName> userBookmarkNames = new(1) {userBookmarkName7};
        return userBookmarkNames;
    }

    private static List<UserBookmarkTitle> GetTitleBookmarksForUser3ForTest()
    {
        UserBookmarkTitle userBookmarkTitle7 = new UserBookmarkTitle() {Tconst="tt22222222", Note="my title note3", Userid=3};
        List<UserBookmarkTitle> userBookmarkTitles = new(1) {userBookmarkTitle7};
        return userBookmarkTitles;
    }
}
