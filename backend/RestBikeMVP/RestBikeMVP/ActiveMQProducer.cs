using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace RestBikeMVP
{
    class ActiveMQProducer
    {
        static void test()
        {
            // URI de connexion au broker ActiveMQ
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");

            // Créer une fabrique de connexion
            IConnectionFactory connectionFactory = new ConnectionFactory(connecturi);

            // Créer une connexion
            IConnection connection = connectionFactory.CreateConnection();
            connection.Start();

            // Créer une session
            ISession session = connection.CreateSession();

            // Cibler ou créer une queue dynamiquement
            IDestination destination = session.GetQueue("itinerary");

            // Créer un producteur pour envoyer des messages
            IMessageProducer producer = session.CreateProducer(destination);

            // Configurer le producteur
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            // Envoyer un message texte
            ITextMessage message = session.CreateTextMessage("Message pour itinerary");
            producer.Send(message);

            Console.WriteLine("Message envoyé vers la queue dynamicQueue.");
        }
    }
}
