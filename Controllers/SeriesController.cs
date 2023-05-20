using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetSPA.Data;
using NetSPA.Models;
using System.Linq;
using System.Security.Claims;
using NetSPA.Repositories;

namespace NetSPA.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SeriesController : ControllerBase
{
    private ILogger<SeriesController> _logger;
    private ApplicationDbContext _context;
    private TrackerRepository _trackerRepo;
    
    public SeriesController(ILogger<SeriesController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
        _trackerRepo = new TrackerRepository(context);
    }

    //  Get series being tracked for user.
    [HttpGet]
    [Route("Get")]
    public IEnumerable<Series> Get()
    {
        var distinctSeries = _context.SeriesTrackings
            .Where
            (
                u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
            )
            .Join(
                _context.Series,
                st => st.SeriesId,
                series => series.Id,
                (st, series) => series
            )
            .AsEnumerable().Distinct();
        return distinctSeries;
    }

    //  Get Episodes for specific series.
    [HttpGet]
    public IEnumerable<Episode> GetEpisodes(int seriesId)
    {
        var distinctEpisodes = _context.EpisodeStatus
            .Where(
                e => e.SeriesId == seriesId && 
                e.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
            ).Join (
                _context.Episodes,
                st => st.EpisodeId,
                episode => episode.Id,
                (st, episode) => episode
            ).Distinct().AsEnumerable();

        return distinctEpisodes;
    }

    //  Delete a series from being tracked.
    [HttpGet]
    [Route("RemoveSeries/{seriesId}")]
    public bool RemoveSeries(int seriesId)
    {
        Console.WriteLine(seriesId);
        if (_context.EpisodeStatus
            .Any
            (
                u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                u.SeriesId == seriesId
            )
        )
        {
            _context.EpisodeStatus
                .RemoveRange
                (
                    _context.EpisodeStatus
                        .Where
                        (
                            u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                            u.SeriesId == seriesId
                        )
                );
        }
        var _seriesTrackingObject = _context.SeriesTrackings
                .FirstOrDefault
                (
                    u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                    u.SeriesId == seriesId
                );
        Console.WriteLine(seriesId);
        if (_seriesTrackingObject != null)
        {
            _context.SeriesTrackings.Remove(_seriesTrackingObject);
        }
        _context.SaveChanges();
        return true;
    }

    //  Update a specific episode.
    //  Note to self, I've just been reading through the API documentation for the metadata we
    //  will be using, I think I can drop seriesId off the episode tracking table.
    [HttpPost]
    public void UpdateEpisode(int episodeId, int seriesId)
    {
        var episode = _context.EpisodeStatus
            .FirstOrDefault
            (
                u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                u.EpisodeId == episodeId
            );

        if (episode == null)
        {
            episode = new EpisodeStatus() {
                EpisodeId = episodeId,
                SeriesId = seriesId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.EpisodeStatus.Add(episode);
        }
        else
        {
            _context.EpisodeStatus.Remove(episode);
        }

        _context.SaveChanges();
    }


    [HttpGet]
    [Route("SearchSeries/{series}")]
    public async Task<IActionResult> SearchSeries(string series)
    {
        List<Series> _seriesList = new List<Series>();
        await foreach (TmdbSeries _series in _trackerRepo.SearchSeries(series))
        {
            //  This I'm planning to map to a DTO, but doing it like this for now so I can test,
            //  battery is about to die with loadshedding.
            _seriesList.Add(new Series
            {   Id = _series.Id, 
                Title = _series.Name,
                Description = _series.Overview,
                BannerUrl = $"https://image.tmdb.org/t/p/original/{_series.PosterPath}"
            });
        }

        return new JsonResult(_seriesList);
    }

    [HttpGet]
    [Route("Watch/{id}")]
    public async Task<IActionResult> Watch(int id)
    {
        var seriesTracking = _context.SeriesTrackings
            .FirstOrDefault(
                s => s.SeriesId == id &&
                s.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
        );
        
        if (seriesTracking != null)
        {
            return new JsonResult(StatusCodes.Status200OK);
        }

        var series = _context.Series.FirstOrDefault(s => s.Id == id);
        //  Only pull the meta data if we don't already have it.
        //  I should add a property to series though so I can check if there are new
        //  episodes coming though.
        if (series == null)
        {
            //  I need to handle not founds here.
            await _trackerRepo.PullSeriesMeta(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        // This needs to move into a repository.
        _context.SeriesTrackings.Add(new SeriesTracking(){
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            SeriesId = id,
        });
        _context.SaveChanges();
        return new JsonResult("");
    }
}