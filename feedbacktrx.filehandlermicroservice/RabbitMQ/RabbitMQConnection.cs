using RabbitMQ.Client;

namespace feedbacktrx.filehandlermicroservice.RabbitMQ
{
    public class RabbitMQConnection
    {
        public readonly string Hostname;
        public readonly string Username;
        public readonly string Password;
        private IConnection _connection;

        public RabbitMQConnection(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
        }

        public IModel CreateModel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                var factory = new ConnectionFactory
                {
                    HostName = Hostname,
                    UserName = Username,
                    Password = Password
                };
                _connection = factory.CreateConnection();
            }

            return _connection.CreateModel();
        }

        public void Close()
        {
            _connection?.Close();
        }
    }
}
