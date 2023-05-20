using NetSPA.Models;
using NetSPA.Data;
using Newtonsoft.Json;

namespace NetSPA.Repositories;

public class TrackerRepository : ITrackerRepository
{

    private ApplicationDbContext _context;
    public TrackerRepository(ApplicationDbContext context)
    {
        _context = context;
    }


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

    
    public async Task<bool> PullSeriesMeta(int seriesId, string userId)
    {
        
        string httpEndPoint = $"https://api.themoviedb.org/3/tv/{seriesId}";
        
        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(httpEndPoint);
            if (response.IsSuccessStatusCode)
            {
                string responseData = response.Content.ReadAsStringAsync().Result;
                Root root = JsonConvert.DeserializeObject<Root>(responseData);
                
                Series series = new Series() {
                    Id = seriesId,
                    Title = root.Name,
                    Description = root.Overview,
                    BannerUrl = $"https://image.tmdb.org/t/p/original/{root.PostPath}"
                };

                _context.Series.Add(series);
                foreach(var season in root.Seasons)
                {
                    for (int i = 1; i < season.EpisodeCount; i++)
                    {
                        Episode episode = new Episode() {
                            SeriesId = seriesId,
                            EpisodeNumber = i,
                            SeasonNumber = season.SeasonNumber,
                        };
                        _context.Episodes.Add(episode);
                    }
                }

            }
        }
        _context.SaveChanges();
        return true;
    }
}