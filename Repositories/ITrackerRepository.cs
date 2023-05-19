using NetSPA.Models;

namespace NetSPA.Repositories;

public interface ITrackerRepository 
{
    IAsyncEnumerable<TmdbSeries> SearchSeries(string seriesName);
}