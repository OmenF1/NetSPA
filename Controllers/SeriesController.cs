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

    [HttpGet]
    public IEnumerable<Series> Get()
    {
        var distinctSeries = _context.EpisodeStatus.Where(u => u.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            .Join(
                _context.Series,
                st => st.SeriesId,
                series => series.Id,
                (st, series) => series
            )
            .Distinct()
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
}