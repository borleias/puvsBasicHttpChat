using System.Net.Http.Json;
using Data;

namespace Client;

/// <summary>
/// A client for the simple web server
/// </summary>
public class ChatClient
{
    /// <summary>
    /// The HTTP client to be used throughout
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// The alias of the user
    /// </summary>
    private readonly string alias;

    /// <summary>
    /// The cancellation token source for the listening task
    /// </summary>
    readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    /// <param name="alias">The alias of the user.</param>
    /// <param name="serverUri">The server URI.</param>
    public ChatClient(string alias, Uri serverUri)
    {
        this.alias = alias;
        this.httpClient = new HttpClient();
        this.httpClient.BaseAddress = serverUri;
    }

    /// <summary>
    /// Connects this client to the server.
    /// </summary>
    /// <returns>True if the connection could be established; otherwise False</returns>
    public async Task<bool> Connect()
    {
        // create and send a welcome message
        var message = new ChatMessage { Sender = this.alias, Content = $"Hi, I joined the chat!" };
        var response = await this.httpClient.PostAsJsonAsync("/messages", message);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Sends a new message into the chat.
    /// </summary>
    /// <param name="content">The message content as text.</param>
    /// <returns>True if the message could be send; otherwise False</returns>
    public async Task<bool> SendMessage(string content)
    {
        // creates the message and sends it to the server
        var message = new ChatMessage { Sender = this.alias, Content = content };
        var response = await this.httpClient.PostAsJsonAsync("/messages", message);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Listens for messages until this process is cancelled by the user.
    /// </summary>
    public async Task ListenForMessages()
    {
        var cancellationToken = this.cancellationTokenSource.Token;

        // run until the user request the cancellation
        while (true)
        {
            try
            {
                // listening for messages. possibly waits for a long time.
                var message = await this.httpClient.GetFromJsonAsync<ChatMessage>($"/messages?id={this.alias}", cancellationToken);

                // if a new message was received notify the user
                if (message != null)
                {
                    this.OnMessageReceived(message.Sender, message.Content);
                }
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // catch the cancellation 
                this.OnMessageReceived("Me", "Leaving the chat");
                break;
            }
        }
    }

    /// <summary>
    /// Cancels the loop for listening for messages.
    /// </summary>
    public void CancelListeningForMessages()
    {
        // signal the cancellation request
        this.cancellationTokenSource.Cancel(); 
    }

    // Enabled the user to receive new messages. The assigned delegated is called when a new message is received.
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Called when a message was received and signal this to the user using the MessageReceived event.
    /// </summary>
    /// <param name="sender">The alias of the sender.</param>
    /// <param name="message">The containing message as text.</param>
    protected virtual void OnMessageReceived(string sender, string message)
    {
        this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs { Sender = sender, Message = message });
    }
}