using Microsoft.AspNetCore.Mvc;

namespace Movies.Application.Common.Interfaces;

public interface IRatingsHandler
{
    ObjectResult RateTitle(string titleId, string key, int rate, string endpointName);
    public ObjectResult UpdateRate(string titleId, int rate, string endpointName);
    public ObjectResult DeleteRating(string titleId, string endpointName);
}
