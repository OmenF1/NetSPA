using Newtonsoft.Json;

namespace NetSPA.Models;
public class TmdbSeries
{
    [JsonIgnore]
    public bool Adult { get; set; }
    [JsonIgnore]
    public string BackdropPath { get; set; }
    [JsonIgnore]
    public List<int> GenreIds { get; set; }
    public int Id { get; set; }
    public List<string> OriginCountry { get; set; }
    [JsonIgnore]
    public string OriginalLanguage { get; set; }
    [JsonIgnore]
    public string OriginalName { get; set; }
    public string Overview { get; set; }
    [JsonIgnore]
    public double Popularity { get; set; }
    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }
    [JsonIgnore]
    public DateTime FirstAirDate { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public double VoteAverage { get; set; }
    [JsonIgnore]
    public int VoteCount { get; set; }
}