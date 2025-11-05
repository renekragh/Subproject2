using Movies.Application.Features.Titles.Models;

namespace Movies.Application.Common.Interfaces;

public interface ITitlesHandler
{
    object RetrieveTitles(int page, int pageSize);
    TitleModel? RetrieveTitle(string id);
    object FindTitles(string name, int page, int pageSize);
}
