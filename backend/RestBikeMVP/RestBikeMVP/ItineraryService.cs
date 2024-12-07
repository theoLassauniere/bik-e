using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RoutingService.Models;

namespace RestBikeMVP
{
    public class ItineraryService
    {
        private const string jcDecauxApiKey = "apiKey=0fe07ec8bd4a1243fe5b004053cac6f992d26218";
        private const string openRouteServiceApiKey = "api_key=5b3ce3597851110001cf6248ddfe5c94ab474310b7d3d01b0d8fc226";
        private const string openRouteServiceUrl = "https://api.openrouteservice.org/v2/directions/";
        private const string getBaseUrl = "https://api.jcdecaux.com/vls/v3/";

        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<List<Response>> ComputeItineraryWithSteps(
            GeoCoordinate origin,
            Station firstStation,
            Station lastStation,
            GeoCoordinate destination, 
            List<Station> itineraryStations)
        {
            ItineraryService itineraryService = new ItineraryService();

            // First we create all the positionnal objects
            GeoCoordinate firstStationPosition = new GeoCoordinate(firstStation.Position.Latitude, firstStation.Position.Longitude);
            GeoCoordinate lastStationPosition = new GeoCoordinate(lastStation.Position.Latitude, lastStation.Position.Longitude);
            List<GeoCoordinate> positions = new List<GeoCoordinate>();
            foreach (Station station in itineraryStations)
            {
                positions.Add(new GeoCoordinate(station.Position.Latitude, station.Position.Longitude));
            }

            // We can add the first two steps Origin -> firstStation (foot), firstStation -> firstIntermediateStation (bike)
            List<Response> itinerary = new List<Response> { 
                itineraryService.GetItinerary(origin, firstStationPosition, true).Result,
                itineraryService.GetItinerary(firstStationPosition, positions[0], false).Result};

            if (positions.Count() > 1)
            {
                for (int i = 1; i < positions.Count() - 1; i++)
                {
                    // If two following stations are in the same contract, we bike
                    itinerary.Add(itineraryStations[i].ContractName == itineraryStations[i+1].ContractName ?
                        itineraryService.GetItinerary(positions[i], positions[i + 1], false).Result :
                        itineraryService.GetItinerary(positions[i], positions[i+1], true).Result);
                }
            }
            return itinerary;
        }

        public static async Task<List<Response>> ComputeItinerary(GeoCoordinate origin, Station firstStation, Station lastStation, GeoCoordinate destination)
        {
            ItineraryService itineraryService = new ItineraryService();
            GeoCoordinate firstStationPosition = new GeoCoordinate(firstStation.Position.Latitude, firstStation.Position.Longitude);
            GeoCoordinate lastStationPosition = new GeoCoordinate(lastStation.Position.Latitude, lastStation.Position.Longitude);

            if (itineraryService.isWorthToGoByFoot(origin, destination, firstStationPosition, lastStationPosition))
            {
                return new List<Response> { itineraryService.GetItinerary(origin, destination, true).Result };
            }

            Console.WriteLine(itineraryService.GetItinerary(origin, firstStationPosition, true).Result);

            return new List<Response> {
                itineraryService.GetItinerary(origin, firstStationPosition, true).Result,
                itineraryService.GetItinerary(firstStationPosition, lastStationPosition, false).Result,
                itineraryService.GetItinerary(lastStationPosition, destination, true).Result};
        }
        public bool isWorthToGoByFoot(
            GeoCoordinate origin,
            GeoCoordinate destination,
            GeoCoordinate firstStation,
            GeoCoordinate lastStation)
        {
            // If both stations are the same, we can skip because it is worth to go by foot
            if (firstStation.Equals(lastStation)) { return true; }

            // Origin -> destination by foot
            double originToDestinationByFoot = GetTravelDuration(origin, destination, true).Result;

            // Origin -> firstStation (foot) -> lastStation (bike) -> destination (foot)
            double originToDestinationByBike = GetTravelDuration(origin, firstStation, true).Result
                + GetTravelDuration(firstStation, lastStation, false).Result
                + GetTravelDuration(lastStation, destination, true).Result;

            return originToDestinationByFoot < originToDestinationByBike;
        }

        private async Task<double> GetTravelDuration(GeoCoordinate origin, GeoCoordinate destination, bool profile)
        {
            // We assume profile is true means to go by foot
            string mode = profile ? "foot-walking?" : "cycling-road?";
            string url = openRouteServiceUrl + mode + openRouteServiceApiKey
                + "&start=" + origin.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + origin.Latitude.ToString(CultureInfo.InvariantCulture)
                + "&end=" + destination.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + destination.Latitude.ToString(CultureInfo.InvariantCulture);

            // Call to openRouteService to get the duration
            HttpResponseMessage getResponse = await httpClient.GetAsync(url);
            var responseContent = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
            var propertiesField = responseContent.RootElement.GetProperty("features")[0].GetProperty("properties");
            var properties = JsonSerializer.Deserialize<Properties>(propertiesField);

            return properties.Summary.Duration;
        }
        public async Task<Response> GetItinerary(GeoCoordinate origin, GeoCoordinate destination, bool profile)
        {         
            // We assume profile is true means to go by foot
            string mode = profile ? "foot-walking?" : "cycling-road?";
            string urlToGetItinerary = openRouteServiceUrl + mode + openRouteServiceApiKey
                + "&start=" + origin.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + origin.Latitude.ToString(CultureInfo.InvariantCulture)
                + "&end=" + destination.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + destination.Latitude.ToString(CultureInfo.InvariantCulture);

            // Call to openRouteService to get itinerary
            HttpResponseMessage getResponse = await httpClient.GetAsync(urlToGetItinerary);
            var responseContent = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
            var featuresField = responseContent.RootElement.GetProperty("features")[0];
            var properties = JsonSerializer.Deserialize<Response>(featuresField);

            return properties;
        }
    }
}
