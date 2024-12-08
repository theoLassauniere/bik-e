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
using static System.Collections.Specialized.BitVector32;
using Apache.NMS.ActiveMQ;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;

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
        ServerResponse IService1.GetInstructions(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            // URI de connexion au broker ActiveMQ
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");

            // Créer une connexion
            IConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
            IConnection connection = connectionFactory.CreateConnection();
            connection.Start();

            // Créer une session
            ISession session = connection.CreateSession();

            // Cibler ou créer une queue dynamiquement
            IDestination itineraryQueue = session.GetQueue("itinerary");

            // Créer un producteur pour envoyer des messages
            IMessageProducer producer = session.CreateProducer(itineraryQueue);

            // Configurer le producteur
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            // Creation of origin and destination
            GeoCoordinate origin = new GeoCoordinate(originLatitude, originLongitude);
            GeoCoordinate destination = new GeoCoordinate(destinationLatitude, destinationLongitude);

            Task<ServerResponse> baseContract = getItinerary(origin, destination);

            // Serializer la réponse
            string jsonMessage = JsonSerializer.Serialize(baseContract.Result);

            // Envoyer le message dans la queue
            ITextMessage message = session.CreateTextMessage(jsonMessage);
            producer.Send(message);

            Console.WriteLine("JSON message sent to queue.");
            return baseContract.Result;
        }

        private async Task<ServerResponse> getItinerary(GeoCoordinate origin, GeoCoordinate destination)
        {
            // Call the JCDecaux api to retreive all stations
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/stations?" + jcDecauxApiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();

            // Convert the response to a list of Station opened and connected
            List<Station> stationsResponse = JsonSerializer.Deserialize<List<Station>>(responseContent).Where(station =>
                station.Connected && station.Status == "OPEN"
            ).ToList();

            // We compute first station and last station
            Station nearestStationFromOrigin = StationService.FindNearestStationFromPoint(stationsResponse, origin);
            Station nearestStationFromDestination = StationService.FindNearestStationFromPoint(stationsResponse, destination);

            List<OpenRouteServiceResponse> itinerary;
            List<Station> allItineraryStations = new List<Station> { nearestStationFromOrigin };

            // We use the if/else bloc for more readability
            if (nearestStationFromOrigin.ContractName != nearestStationFromDestination.ContractName)
            {
                List<Station> stations = StationService.ComputeAllStationsInItinerary(stationsResponse, nearestStationFromOrigin, nearestStationFromDestination);
                stations.ForEach(station => allItineraryStations.Add(station));
                itinerary = ItineraryService.ComputeItineraryWithSteps(
                    origin, nearestStationFromOrigin, nearestStationFromDestination, destination, stations).Result;
            } else
            {
                itinerary = ItineraryService.ComputeItinerary(
                origin, nearestStationFromOrigin, nearestStationFromDestination, destination).Result;

            }
            allItineraryStations.Add(nearestStationFromDestination);
            
            return BuildServerResponse(itinerary, allItineraryStations);
        }

        public ServerResponse BuildServerResponse(List<OpenRouteServiceResponse> itinerary, List<Station> stations)
        {
            ServerResponse response = new ServerResponse();

            foreach (OpenRouteServiceResponse properties in itinerary)
            {
                properties.Properties.Segments.ForEach(segment => response.Segments.Add(segment));
                properties.Properties.WayPoints.ForEach(wayPoint => response.WayPoints.Add(wayPoint));
                properties.Geometry.Coordinates.ForEach(coordinate => response.Coordinates.Add(coordinate));
            }
            stations.ForEach(station => response.Stations.Add(station.Position.ToString()));

            return response;
        }
    }
}
