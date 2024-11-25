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

namespace RestBikeMVP
{
    public class Service1 : IService1
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string apiKey = "apiKey=0fe07ec8bd4a1243fe5b004053cac6f992d26218";
        private string getBaseUrl = "https://api.jcdecaux.com/vls/v3/";
        public string GetInstructions(string origin)
        {
            Task<string> baseContract = getContractFromOrigin(origin);
            return baseContract.Result;
        }

        private async Task<string> getContractFromOrigin(string origin)
        {
            // Call the JCDecaux api to retreive all stations
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/stations?" + apiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();

            // Convert the response to a list of Station
            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseContent);

            string res = "";

            // Process all the stations to find the nearest one
            foreach (Station station in stations)
            {
                res += station.Name;
            }

            return res;
        }
    }
}
