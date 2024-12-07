using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using static RestBikeMVP.CorsBehavior;

namespace RestBikeMVP
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the service URI
            Uri baseAddress = new Uri("http://localhost:8733/Design_Time_Addresses/RestBikeMVP/Service1/");

            // Create the ServiceHost
            using (var host = new MyServiceHost(typeof(Service1), baseAddress))
            {

                try
                {
                    // Add a service endpoint with webHttpBinding
                    var endpoint = host.AddServiceEndpoint(typeof(IService1), new WebHttpBinding(), "");

                    // Ensure only one instance of WebHttpBehavior is added
                    var webHttpBehavior = new WebHttpBehavior();
                    if (!endpoint.EndpointBehaviors.Contains(webHttpBehavior))
                    {
                        endpoint.EndpointBehaviors.Add(webHttpBehavior);
                    }

                    // Add CORS behavior
                    endpoint.EndpointBehaviors.Add(new CorsBehavior());

                    // Open the ServiceHost to start listening for messages
                    host.Open();

                    Console.WriteLine("The service is ready at {0}", baseAddress);
                    Console.WriteLine("Press <Enter> to stop the service.");
                    Console.ReadLine();

                    host.Close();
                }
                catch (CommunicationException ce)
                {
                    Console.WriteLine("An exception occurred: {0}", ce.Message);
                    host.Abort();
                }
            }
        }
    }
}
