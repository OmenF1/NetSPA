namespace NetSPA.Models;

public class EpisodeStatus 
{
    public int Id {get;set;}
    public string UserId {get;set;}
    public int EpisodeId {get;set;}
    public int SeriesId {get;set;}

    public Episode Episode {get;set;}
    public Series Series {get;set;}
}