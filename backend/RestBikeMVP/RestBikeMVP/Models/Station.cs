using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RestBikeMVP.Models;

namespace RestBikeMVP
{
    public class Station
    {
        [JsonPropertyName("number")] public int Number {  get; set; }
        [JsonPropertyName("contractName")] public string ContractName { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("address")] public string Address { get; set; }
        [JsonPropertyName("position")] public Position Position { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("connected")] public Boolean Connected { get; set; }
    }
}
