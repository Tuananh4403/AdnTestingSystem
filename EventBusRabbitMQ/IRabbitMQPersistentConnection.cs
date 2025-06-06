using RabbitMQ.Client;
using System;

namespace eBooking.EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IConnection GetConnection();
    }
}
