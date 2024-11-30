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
using System.Device.Location;
using System.Globalization;
using RoutingService.Models;

namespace RestBikeMVP
{
    public class Service1 : IService1
    {
        // Constants definition
        private const string jcDecauxApiKey = "apiKey=0fe07ec8bd4a1243fe5b004053cac6f992d26218";
        private const string openRouteServiceApiKey = "api_key=5b3ce3597851110001cf6248ddfe5c94ab474310b7d3d01b0d8fc226";
        private const string openRouteServiceUrl = "https://api.openrouteservice.org/v2/directions/";
        private const string getBaseUrl = "https://api.jcdecaux.com/vls/v3/";

        private static readonly HttpClient httpClient = new HttpClient();
        string IService1.GetInstructions(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            // Creation of origin and destination
            GeoCoordinate origin = new GeoCoordinate(originLatitude, originLongitude);
            GeoCoordinate destination = new GeoCoordinate(destinationLatitude, destinationLongitude);

            Task<string> baseContract = getItinerary(origin, destination);
            return baseContract.Result;
        }

        private async Task<string> getItinerary(GeoCoordinate origin, GeoCoordinate destination)
        {
            // Call the JCDecaux api to retreive all stations
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/stations?" + jcDecauxApiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();

            // Convert the response to a list of Station opened and connected
            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseContent).Where(station =>
                station.Connected && station.Status == "OPEN"
            ).ToList();

            // We compute first station and last station
            Station nearestStationFromOrigin = StationService.FindNearestStationFromPoint(stations, origin);
            Station nearestStationFromDestination = StationService.FindNearestStationFromPoint(stations, destination);

            List<Properties> itinerary;

            // We use the if/else bloc for more readability
            if (nearestStationFromOrigin.ContractName != nearestStationFromDestination.ContractName)
            {
                List<Station> itineraryStations = StationService.ComputeAllStationsInItinerary(stations, nearestStationFromOrigin, nearestStationFromDestination);
                itinerary = ItineraryService.ComputeItineraryWithSteps(
                    origin, nearestStationFromOrigin, nearestStationFromDestination, destination, itineraryStations).Result;
            } else
            {
                itinerary = ItineraryService.ComputeItinerary(
                origin, nearestStationFromOrigin, nearestStationFromDestination, destination).Result;
            }

            List<Segment> segments = new List<Segment>();

            foreach (Properties props in  itinerary)
            {
                props.Segments.ForEach(segment => segments.Add(segment));
            }

            return JsonSerializer.Serialize(segments, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
