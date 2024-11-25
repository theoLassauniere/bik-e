using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Net.WebRequestMethods;

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
            string getUrlForContractName = "https://api.jcdecaux.com/vls/v3/contracts?" + apiKey;
            HttpResponseMessage getResponse = await httpClient.GetAsync(getUrlForContractName);
            string responseContent = await getResponse.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
