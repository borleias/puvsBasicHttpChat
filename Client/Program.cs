namespace Client;

/// <summary>
/// A most basic chat client for the console
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var serverUri = new Uri("http://localhost:5000");

        // query the user for a name
        Console.Write("Geben Sie Ihren Namen ein: ");
        var sender = Console.ReadLine() ?? Guid.NewGuid().ToString();
        Console.WriteLine();

        // create a new client and connect the event handler for the received messages
        var client = new ChatClient(sender, serverUri);
        client.MessageReceived += MessageReceivedHandler;

        // connect to the server and start listening for messages
        var connectTask = await client.Connect();
        var listenTask = client.ListenForMessages();

        // query the user for messages to send or the exit command
        while (true)
        {
            Console.Write("Geben Sie Ihre Nachricht ein (oder 'exit' zum Beenden): ");
            var content = Console.ReadLine() ?? string.Empty;

            // cancel the listening task and exit the loop
            if (content.ToLower() == "exit")
            {
                client.CancelListeningForMessages();
                break;
            }

            Console.WriteLine($"Sending message: {content}");

            // send the message and display the result
            if (await client.SendMessage(content))
            {
                Console.WriteLine("Message sent successfully.");
            }
            else
            {
                Console.WriteLine("Failed to send message.");
            }
        }

        // wait for the listening for new messages to end
        await Task.WhenAll(listenTask);

        Console.WriteLine("\nGood bye...");
    }

    /// <summary>
    /// Helper method to display the newly received messages.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MessageReceivedEventArgs"/> instance containing the event data.</param>
    static void MessageReceivedHandler(object? sender, MessageReceivedEventArgs e)
    {
        Console.WriteLine($"\nReceived new message from {e.Sender}: {e.Message}");
    }
}