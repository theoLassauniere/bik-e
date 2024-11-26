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

namespace RestBikeMVP
{
    public class Service1 : IService1
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string jcDecauxApiKey = "apiKey=0fe07ec8bd4a1243fe5b004053cac6f992d26218";
        private const string openRouteServiceApiKey = "api_key=5b3ce3597851110001cf6248ddfe5c94ab474310b7d3d01b0d8fc226";
        private const string openRouteServiceUrl = "https://api.openrouteservice.org/v2/directions/";
        private string getBaseUrl = "https://api.jcdecaux.com/vls/v3/";
        string IService1.GetInstructions(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            // Creation of origin and destination
            GeoCoordinate origin = new GeoCoordinate(originLatitude, originLongitude);
            GeoCoordinate destination = new GeoCoordinate(destinationLatitude, destinationLongitude);

            Task<string> baseContract = getItinary(origin, destination);
            return baseContract.Result;
        }

        private async Task<string> getItinary(GeoCoordinate origin, GeoCoordinate destination)
        {
            // Call the JCDecaux api to retreive all stations
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/stations?" + jcDecauxApiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();

            // Convert the response to a list of Station
            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseContent);

            
            Station nearestStationFromOrigin = FindNearestStationFromOriginAmongAllStations(stations, origin);
            if (isWorthToGoByFoot(origin, destination, nearestStationFromOrigin))
            {
                return GetFootItinary(origin, destination).Result;
            }
            // We compute the nearest station from destination and all coordinates
            Station nearestStationFromDestination = FindNearestStationFromOriginAmongAllStations(stations, destination);
            GeoCoordinate nearestStationFromOriginCoordinates = new GeoCoordinate(nearestStationFromOrigin.Position.Latitude, nearestStationFromOrigin.Position.Longitude);
            GeoCoordinate nearestStationFromDestinationCoordinates = new GeoCoordinate(nearestStationFromDestination.Position.Latitude, nearestStationFromDestination.Position.Longitude);
            
            // We return the final itinary
            return GetFootItinary(origin, nearestStationFromOriginCoordinates).Result
                + GetBikeItinary(nearestStationFromOriginCoordinates, nearestStationFromDestinationCoordinates).Result
                + GetFootItinary(nearestStationFromDestinationCoordinates, destination).Result;
        }

        private async Task<string> GetFootItinary(GeoCoordinate origin, GeoCoordinate destination)
        {
            string urlToGetItinary = openRouteServiceUrl + "foot-walking?" 
                + openRouteServiceApiKey
                + "&start=" + origin.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + origin.Latitude.ToString(CultureInfo.InvariantCulture)
                + "&end=" + destination.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + destination.Latitude.ToString(CultureInfo.InvariantCulture);
            HttpResponseMessage getResponse = await httpClient.GetAsync(urlToGetItinary);
            return await getResponse.Content.ReadAsStringAsync();
        }

        private async Task<string> GetBikeItinary(GeoCoordinate origin, GeoCoordinate destination)
        {
            string urlToGetItinary = openRouteServiceUrl + "cycling-regular?"
                + openRouteServiceApiKey
                + "&start=" + origin.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + origin.Latitude.ToString(CultureInfo.InvariantCulture)
                + "&end=" + destination.Longitude.ToString(CultureInfo.InvariantCulture) + ", " + destination.Latitude.ToString(CultureInfo.InvariantCulture);
            HttpResponseMessage getResponse = await httpClient.GetAsync(urlToGetItinary);
            return await getResponse.Content.ReadAsStringAsync();
        }

        private bool isWorthToGoByFoot(GeoCoordinate origin, GeoCoordinate destination, Station nearestStationFromOrigin)
        {
            double walkingSpeed = 4; // We assume 4 is in km per hour
            double timeToTravelToDestination = origin.GetDistanceTo(destination) / walkingSpeed;
            double timeToTravelToNearestStaion = origin.GetDistanceTo(new GeoCoordinate(
                nearestStationFromOrigin.Position.Latitude, nearestStationFromOrigin.Position.Longitude)
                ) / walkingSpeed;
            return timeToTravelToDestination < timeToTravelToNearestStaion;
        }

        private Station FindNearestStationFromOriginAmongAllStations(List<Station> stations, GeoCoordinate origin)
        { 
            // Initialize a station 
            Station nearestStation = null;
            double resDistance = double.MaxValue;

            // Compute all stations to find the nearest from origin
            foreach (Station station in stations)
            {
                double distanceFromOrigin = origin.GetDistanceTo(new GeoCoordinate(station.Position.Latitude, station.Position.Longitude));
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
