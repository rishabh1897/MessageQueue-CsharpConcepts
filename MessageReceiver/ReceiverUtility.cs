using Azure.Messaging.ServiceBus;

namespace MessageReceiver
{
    public class ReceiverUtility
    {
        private readonly ServiceBusClient _client;


        // the sender used to publish messages to the queue
        private readonly ServiceBusReceiver _receiver;
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly ServiceBusClientOptions _options;

        public ReceiverUtility(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            _options = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            _client = new ServiceBusClient(_connectionString, _options);
            _receiver = _client.CreateReceiver(_queueName);
        }

        public async Task<int> ReceiveMessageAsync(string destinationFolder)
        {
            //delay is intention to send message first in the queue
            Thread.Sleep(1000);
            int noOfMessageProcessed = 0;
            try
            {
                while (true)
                {   
                    var isMessage = await _receiver.PeekMessageAsync();

                    if (isMessage != null)
                    {
                        var message = await _receiver.ReceiveMessageAsync();
                        await ProcessFileMessageAsync(message, destinationFolder);
                        Console.WriteLine("Processing Message");
                        await _receiver.CompleteMessageAsync(message);
                        noOfMessageProcessed++;
                    }
                    else
                    {
                        Console.WriteLine("No more Message Available");
                        break;
                    }
                }
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _receiver.DisposeAsync();
                await _client.DisposeAsync();
            }
            return noOfMessageProcessed;

        }
        private async Task ProcessFileMessageAsync(ServiceBusReceivedMessage message, string destinationFolder)
        {
            string fileName = Path.GetFileNameWithoutExtension(message.Subject) + "_processed";
            string filePath = Path.Combine(destinationFolder, $"{fileName}.docx");

            await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(message.Body.ToString()));

            // Perform further processing on the file as needed
            // For example, you can call a method to handle the file:
            await HandleFileAsync(filePath);
        }

        private async Task HandleFileAsync(string filePath)
        {
            // Implement your logic to process the file here
            await Task.Delay(10); // Simulating file processing time
            Console.WriteLine($"Processed file: {filePath}");
        }
    }
}