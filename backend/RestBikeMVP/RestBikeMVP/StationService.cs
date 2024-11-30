using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestBikeMVP
{
    public class StationService
    {
        public static Station FindNearestStationFromPoint(List<Station> stations, GeoCoordinate origin)
        {
            // Initialize a station 
            Station nearestStation = null;
            double resDistance = double.MaxValue;

            foreach (Station station in stations)
            {
                GeoCoordinate stationPoint = new GeoCoordinate(station.Position.Latitude, station.Position.Longitude);
                double distanceFromOrigin = origin.GetDistanceTo(stationPoint);
                if (distanceFromOrigin < resDistance)
                {
                    resDistance = distanceFromOrigin;
                    nearestStation = station;
                }
            }
            return nearestStation;
        }

        public static Station FindStationBetweenOriginAndDestination(List<Station> stations, GeoCoordinate origin, GeoCoordinate destination)
        {
            // Initialize a station 
            Station nearestStation = null;
            double resDistance = double.MaxValue;

            // Compute all stations to find the nearest from origin
            foreach (Station station in stations)
            {
                GeoCoordinate stationPoint = new GeoCoordinate(station.Position.Latitude, station.Position.Longitude);
                double distanceFromOrigin = origin.GetDistanceTo(stationPoint) + stationPoint.GetDistanceTo(destination);
                if (distanceFromOrigin < resDistance)
                {
                    resDistance = distanceFromOrigin;
                    nearestStation = station;
                }
            }
            return nearestStation;
        }

        public static Station FindNearestStationFromDestinationInContract(List<Station> Stations, Station Destination, string ContractName)
        {
            // Initialize a station 
            Station nearestStation = null;
            double resDistance = double.MaxValue;

            // Filter the station list
            List<Station> filteredStations = Stations.Where(station => station.ContractName == ContractName).ToList();

            foreach (Station station in filteredStations)
            {
                if (station.ContractName == ContractName)
                {
                    GeoCoordinate stationPoint = new GeoCoordinate(station.Position.Latitude, station.Position.Longitude);
                    GeoCoordinate destinationPoint = new GeoCoordinate(Destination.Position.Latitude, Destination.Position.Longitude);
                    double distanceFromOrigin = stationPoint.GetDistanceTo(destinationPoint);

                    if (distanceFromOrigin < resDistance)
                    {
                        resDistance = distanceFromOrigin;
                        nearestStation = station;
                    }
                }
            }

            return nearestStation;
        }

        public static List<Station> ComputeAllStationsInItinary(List<Station> Stations, Station origin, Station destination)
        {
            string contractName = origin.ContractName;
            List<Station> itinary = new List<Station>();
            List<Station> filteredStationsInContract = Stations.Where(station => station.ContractName == contractName).ToList();
            List<Station> filteredStationsOutOFContract = Stations.Where(station => station.ContractName != contractName).ToList();
            GeoCoordinate destinationPoint = new GeoCoordinate(destination.Position.Latitude, destination.Position.Longitude);

            while (contractName != destination.ContractName)
            {
                Station nextStationInContract = FindNearestStationFromDestinationInContract(filteredStationsInContract, destination, contractName);
                itinary.Add(nextStationInContract);

                GeoCoordinate stationPoint = new GeoCoordinate(
                    nextStationInContract.Position.Latitude, 
                    nextStationInContract.Position.Longitude);

                Station nextStationOutOfContract = FindStationBetweenOriginAndDestination(filteredStationsOutOFContract, stationPoint, destinationPoint);
                itinary.Add(nextStationOutOfContract);

                contractName = nextStationOutOfContract.ContractName;
            }

            return itinary.Distinct().ToList();
        }
    }
}
