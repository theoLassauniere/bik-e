using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RestBikeMVP
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the service URI
            Uri baseAddress = new Uri("http://localhost:8733/Design_Time_Addresses/RestBikeMVP/Service1/");

            // Create the ServiceHost
            using (ServiceHost host = new ServiceHost(typeof(Service1), baseAddress))
            {
                
                    // Enable metadata publishing
                    ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (smb == null)
                    {
                        smb = new ServiceMetadataBehavior { HttpGetEnabled = true, HttpsGetEnabled = false };
                        host.Description.Behaviors.Add(smb);
                    }

                    // Open the ServiceHost to start listening for messages
                    host.Open();

                    Console.WriteLine("The service is ready at {0}", baseAddress);
                    Console.WriteLine("Press <Enter> to stop the service.");
                    Console.ReadLine();
            }
        }
    }
}
