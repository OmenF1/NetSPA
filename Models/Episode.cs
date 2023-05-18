namespace NetSPA.Models;

public class Episode 
{
    public int Id {get;set;}
    public int SeriesId {get;set;}
    public int EpisodeNumber {get;set;}
    public int SeasonNumber {get;set;}
    public string? Title {get;set;}
    
    public Series Series {get;set;}
    public ICollection<EpisodeStatus> EpisodeStatuses {get;set;}
}