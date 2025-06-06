using RabbitMQ.Client;
using System;

namespace eBooking.EventBusRabbitMQ;
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly object _lock = new object();

        public DefaultRabbitMQPersistentConnection(string hostName, string userName, string password)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                RequestedHeartbeat = TimeSpan.FromSeconds(60), // ✅ Enable Heartbeats
                AutomaticRecoveryEnabled = true,  // ✅ Auto-reconnect
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // ✅ Retry interval
            };
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public bool TryConnect()
        {
            lock (_lock)
            {
                if (!IsConnected)
                {
                    _connection = _connectionFactory.CreateConnection();
                }
            }
            return IsConnected;
        }

        public IConnection GetConnection()
        {
            if (!IsConnected) TryConnect();
            return _connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
