// See https://aka.ms/new-console-template for more information
using MessageReceiver;
using MessageSender;

var cts = new CancellationTokenSource();
CancellationToken token = cts.Token;
Console.WriteLine("Process Start :0");
string connectionString = "Endpoint=sb://queuetasks.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bDYJWajDPrcvBAhBvdAilZHOuYV2OuxfW+ASbJJvqHM=";
string queueName = "myqueue";
Task sendMessage = Task.Factory.StartNew(() =>
{
    _ = SendMessages(connectionString, queueName);
});

Task receiveMessage = Task.Factory.StartNew(() => ReceiveMessages(connectionString, queueName));

Task.WaitAll(sendMessage, receiveMessage);

Console.WriteLine("Process Complete");
Console.ReadLine();

static async Task<int> SendMessages(string connectionString, string queueName)
{
    string sourceFolderPath = @"C:\Users\Rishabh_Singh\source\MentorshipTasks\3-MessgQueue\FolderProcessor\RawData\";
    Console.WriteLine("Sending Messages");
    SenderService sender = new SenderService(connectionString, queueName);
    var message = await sender.SendMessageAsync(sourceFolderPath);
    Console.WriteLine($"Messages has been published to the queue. Name of the file{0}", message.Subject);
    return 0;
}

static async void ReceiveMessages(string connectionString, string queueName)
{
    string destinationFolder = @"C:\Users\Rishabh_Singh\source\MentorshipTasks\3-MessgQueue\FolderProcessor\ProcessedData\";
    Console.WriteLine("Receiving Messages");
    ReceiverService receiver = new ReceiverService(connectionString, queueName);
    var messageCount = await receiver.ReceiveMessageAsync(destinationFolder);
    Console.WriteLine("All Message proccessed.");
    Console.WriteLine($"Number of message Processed:- {messageCount}");

}