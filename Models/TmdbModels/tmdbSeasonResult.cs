using System.Collections.Generic;
using Newtonsoft.Json;

public class Season
{
    [JsonProperty("air_date")]
    public string AirDate { get; set; }

    [JsonProperty("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("overview")]
    public string Overview { get; set; }

    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }

    [JsonProperty("season_number")]
    public int SeasonNumber { get; set; }
}

public class Root
{
    [JsonProperty("seasons")]
    public List<Season> Seasons { get; set; }
    [JsonProperty("name")]
    public string Name {get; set; }
    [JsonProperty("poster_path")]
    public string PostPath {get; set; }
    [JsonProperty("overview")]
    public string Overview {get; set; }
}