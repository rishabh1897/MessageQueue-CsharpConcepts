// See https://aka.ms/new-console-template for more information
using MessageReceiver;
using MessageSender;

var cts = new CancellationTokenSource();
CancellationToken token = cts.Token;
Console.WriteLine("Hello, World!");
string connectionString = "Endpoint=sb://queuetasks.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bDYJWajDPrcvBAhBvdAilZHOuYV2OuxfW+ASbJJvqHM=";
string queueName = "myqueue";
Task sendMessage = Task.Factory.StartNew(() =>
{
    _ = SendMessages(connectionString, queueName);
});

Task reciveMessage = Task.Factory.StartNew(() => ReceiveMessages(connectionString, queueName));

Task.WaitAll(sendMessage, reciveMessage);

Console.WriteLine("Task Completed");
Console.ReadLine();

static async Task<int> SendMessages(string connectionString, string queueName)
{
    string sourceFolderPath = @"C:\Users\Rishabh_Singh\source\MentorshipTasks\3-MessgQueue\FolderProcessor\RawData\";
    Console.WriteLine("Sending Messages");
    SenderUtility sender = new SenderUtility(connectionString, queueName);
    var message = await sender.SendMessageAsync(sourceFolderPath);
    Console.WriteLine($"messages has been published to the queue. Name of the file{0}", message.Subject);
    return 0;
}

static async void ReceiveMessages(string connectionString, string queueName)
{
    string destinationFolder = @"C:\Users\Rishabh_Singh\source\MentorshipTasks\3-MessgQueue\FolderProcessor\ProcessedData\";
    Console.WriteLine("Receiving Messages");
    ReceiverUtility receiver = new ReceiverUtility(connectionString, queueName);
    var messageCount = await receiver.ReceiveMessageAsync(destinationFolder);
    Console.WriteLine("All Message proccessed.");
    Console.WriteLine($"Number of message Processed:- {messageCount}");
    //Console.WriteLine($"Number of message Processed:-" + messageCount);

}