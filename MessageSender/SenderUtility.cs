using Azure.Messaging.ServiceBus;

namespace MessageSender
{
    public class SenderUtility
    {
        private readonly ServiceBusClient _client;


        // the sender used to publish messages to the queue
        private readonly ServiceBusSender _sender;
        readonly int numOfMessages;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly ServiceBusClientOptions _options;

        public SenderUtility(string connectionString, string queueName)
        {

            _connectionString = connectionString;
            _queueName = queueName;
            numOfMessages = 3;
            _options = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            _client = new ServiceBusClient(_connectionString, _options);
            _sender = _client.CreateSender(_queueName);
        }

        public async Task<ServiceBusMessage> SendMessageAsync(string folderPath)
        {
            var message = new ServiceBusMessage();
            try
            {
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    message = new ServiceBusMessage(Convert.ToBase64String(File.ReadAllBytes(file)))
                    {
                        Subject = file
                    };
                    await _sender.SendMessageAsync(message);
                }

            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Cannot find the directory:- {0}", folderPath);
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }
            return message;
        }

    }
}