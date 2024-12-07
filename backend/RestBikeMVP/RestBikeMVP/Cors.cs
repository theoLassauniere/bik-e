using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;

namespace RestBikeMVP
{
    public class Cors : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply.Properties.TryGetValue(HttpResponseMessageProperty.Name, out var httpResponsePropertyObj) &&
                httpResponsePropertyObj is HttpResponseMessageProperty httpResponseProperty)
            {
                httpResponseProperty.Headers["Access-Control-Allow-Origin"] = "*";
                httpResponseProperty.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
                httpResponseProperty.Headers["Access-Control-Allow-Headers"] = "Content-Type, Accept";
            }
            else
            {
                var httpResponse = new HttpResponseMessageProperty();
                httpResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                httpResponse.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                httpResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept");
                reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
            }
        }
    }

    public class CorsBehavior : IServiceBehavior, IEndpointBehavior, IDispatchMessageInspector
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceHostBase.ChannelDispatchers)
            {
                if (endpoint is ChannelDispatcher channelDispatcher)
                {
                    foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new Cors());
                    }
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsBehavior());
        }

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public class MyServiceHost : ServiceHost
        {
            public MyServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
            {
                this.Description.Behaviors.Add(new CorsBehavior());
            }
        }
    }
}
