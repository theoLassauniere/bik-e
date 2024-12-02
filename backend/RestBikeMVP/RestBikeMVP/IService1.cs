using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using RestBikeMVP.Models;
using RoutingService.Models;

namespace RestBikeMVP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(
            Method = "GET", 
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.Wrapped, 
            UriTemplate = "getInstructions?originLatitude={originLatitude}&originLongitude={originLongitude}&destinationLatitude={destinationLatitude}&destinationLongitude={destinationLongitude}")]
        ServerResponse GetInstructions(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude);
    }
}
