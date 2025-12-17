using Movies.Domain.Entities;

namespace Movies.Domain.Interfaces;

public interface INamesRepository
{
    Name GetNameWithRelatedEntities(string id);
}
