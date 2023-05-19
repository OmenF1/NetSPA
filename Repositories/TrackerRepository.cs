using NetSPA.Models;
using Newtonsoft.Json;

namespace NetSPA.Repositories;

public class TrackerRepository : ITrackerRepository
{


    public async IAsyncEnumerable<TmdbSeries> SearchSeries(string seriesName)
    {
        
        string httpEndPoint = $"https://api.themoviedb.org/3/search/tv";
        
        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(httpEndPoint);
            if (response.IsSuccessStatusCode)
            {
                string responseData = response.Content.ReadAsStringAsync().Result;
                TmdbSearchResult searchResult = JsonConvert.DeserializeObject<TmdbSearchResult>(responseData);
                if (searchResult?.Results != null)
                {
                    foreach (TmdbSeries series in searchResult.Results)
                    {
                        yield return series;
                    }
                }
            }
        }
    }
}