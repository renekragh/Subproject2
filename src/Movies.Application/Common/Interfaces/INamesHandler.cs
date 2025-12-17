using Movies.Application.Common.Behaviors;

namespace Movies.Application.Common.Interfaces;

public interface INamesHandler
{
    object GetNames(string endpointName, Paging pagingParams);
    object GetName(string endpointName, string id);
    object FindNames(string endpointName, string name, Paging pagingParams);
}
