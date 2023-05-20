namespace NetSPA.Models;
//  Rename this at a later stage, I can't think of a good name right now.
//  This is a flaw in my design, because until a user marks off a episode on the episode tracking
//  table, there will be no link between that user and the series.

public class SeriesTracking {
    public int Id {get;set;}
    public int SeriesId {get;set;}
    public string UserId {get;set;}
    public int CurrentSeason {get;set;}
    public int CurrentEpisode {get;set;}
}