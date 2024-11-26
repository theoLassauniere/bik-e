using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Net;
using RestBikeMVP.Models;

namespace RestBikeMVP
{
    public class Service1 : IService1
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string apiKey = "apiKey=0fe07ec8bd4a1243fe5b004053cac6f992d26218";
        private string getBaseUrl = "https://api.jcdecaux.com/vls/v3/";
        string IService1.GetInstructions(double latitude, double longitude)
        {
            Position origin = new Position(latitude, longitude);
            Task<string> baseContract = getContractFromOrigin(origin);
            return baseContract.Result;
        }

        private async Task<string> getContractFromOrigin(Position origin)
        {
            // Call the JCDecaux api to retreive all stations
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/stations?" + apiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();

            // Convert the response to a list of Station
            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseContent);

            
            Station nearestStation = FindNearestStation(stations, origin);
            return nearestStation.Position.ToString();
        }

        private Station FindNearestStation(List<Station> stations, Position origin)
        {
            // Initialize a station 
            Station nearestStation = null;
            double resDistance = double.MaxValue;

            // Compute all stations to find the nearest from origin
            foreach (Station station in stations)
            {
                double distanceFromOrigin = 
                    Math.Abs(station.Position.Latitude - origin.Latitude) 
                    + Math.Abs(station.Position.Longitude - origin.Longitude);
                if (distanceFromOrigin < resDistance)
                {
                    resDistance = distanceFromOrigin;
                    nearestStation = station;
                }
            }
            return nearestStation;
        }
    }
}
