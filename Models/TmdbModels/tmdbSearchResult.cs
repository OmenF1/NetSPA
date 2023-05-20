using Newtonsoft.Json;

namespace NetSPA.Models;
public class TmdbSearchResult
{
    public int Page { get; set; }
    public List<TmdbSeries> Results { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}
