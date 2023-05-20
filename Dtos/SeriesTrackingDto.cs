using NetSPA.Models;

namespace NetSPA.Dtos;

public class SeriesTrackingDto : Series
{
    public int CurrentEpisode {get;set;}
    public int CurrentSeason {get;set;}
}