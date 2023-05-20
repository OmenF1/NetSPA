using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetSPA.Data;
using NetSPA.Models;
using System.Linq;
using System.Security.Claims;
using NetSPA.Repositories;
using NetSPA.Dtos;

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
                (st, series) => new SeriesTrackingDto 
                {
                    Id = series.Id,
                    Title = series.Title,
                    BannerUrl = series.BannerUrl,
                    Description = series.Description,
                    CurrentEpisode = st.CurrentEpisode,
                    CurrentSeason = st.CurrentSeason
                }
            )
            .AsEnumerable().Distinct();
        
        return distinctSeries;
    }

    [HttpGet]
    [Route("NextEpisode/{seriesId}")]
    public bool NextEpisode(int seriesId)
    {
        var userCurrent = _context.SeriesTrackings
            .FirstOrDefault(
                u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                u.SeriesId == seriesId
            );

        if (userCurrent == null)
        {
            return false;
        }

        int maxSeasons = _context.Episodes
            .Where (
                s => s.SeriesId == seriesId
            ).Select(
                s => s.SeasonNumber
            ).Max();
        
        int episodesInCurrentSeason = _context.Episodes
            .Where(
                s => s.SeriesId == seriesId && 
                s.SeasonNumber == userCurrent.CurrentSeason
            )
            .Select(s => s.EpisodeNumber)
            .Max();

        if (episodesInCurrentSeason == userCurrent.CurrentEpisode)
        {
            if (maxSeasons > userCurrent.CurrentSeason)
            {
                userCurrent.CurrentEpisode = 1;
                userCurrent.CurrentSeason++;
            }
        }
        else
        {
            userCurrent.CurrentEpisode++;
        }
        _context.SaveChanges();
        return true;
    }


    //  Delete a series from being tracked.
    [HttpGet]
    [Route("RemoveSeries/{seriesId}")]
    public bool RemoveSeries(int seriesId)
    {
        var _seriesTrackingObject = _context.SeriesTrackings
                .FirstOrDefault
                (
                    u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) &&
                    u.SeriesId == seriesId
                );
        if (_seriesTrackingObject != null)
        {
            _context.SeriesTrackings.Remove(_seriesTrackingObject);
        }
        _context.SaveChanges();
        return true;
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
            CurrentEpisode = 1,
            CurrentSeason = 1
        });
        _context.SaveChanges();
        return new JsonResult("");
    }
}