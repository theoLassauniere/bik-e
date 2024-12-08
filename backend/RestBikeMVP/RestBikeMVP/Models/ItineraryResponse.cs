using System.Collections.Generic;
using System;
using System.Drawing;
using RestBikeMVP;
using System.Text.Json.Serialization;
using RestBikeMVP.Models;

namespace RoutingService.Models
{
    public class OpenRouteServiceResponse
    {
        [JsonPropertyName("properties")] public Properties Properties { get; set; }
        [JsonPropertyName("geometry")] public Geometry Geometry { get; set; }

    }
    public class Properties
    {
        [JsonPropertyName("segments")] public List<Segment> Segments { get; set; }
        [JsonPropertyName("way_points")] public List<int> WayPoints { get; set; }
        [JsonPropertyName("summary")]  public Summary Summary { get; set; }
    }
    public class Geometry
    {
        [JsonPropertyName("coordinates")] public List<List<double>> Coordinates { get; set; }
    }
    public class Segment
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")]  public double Duration { get; set; }
        [JsonPropertyName("steps")]  public List<Step> Steps { get; set; }
    }

    public class Step
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
        [JsonPropertyName("type")] public int Type { get; set; }
        [JsonPropertyName("instruction")] public string Instruction { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("way_points")] public List<int> WayPoints { get; set; }
    }

    public class Summary
    {
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
    }
}