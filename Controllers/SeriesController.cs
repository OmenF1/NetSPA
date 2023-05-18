using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetSPA.Data;
using NetSPA.Models;
using System.Linq;
using System.Security.Claims;


namespace NetSPA.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]

public class SeriesController : Controller
{
    private ILogger<SeriesController> _logger;
    private ApplicationDbContext _context;
    
    public SeriesController(ILogger<SeriesController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    //  Get series being tracked for user.
    [HttpGet]
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
            .AsEnumerable();
        
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
    [HttpPost]
    public void RemoveSeries(int seriesId)
    {
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
        }
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
}