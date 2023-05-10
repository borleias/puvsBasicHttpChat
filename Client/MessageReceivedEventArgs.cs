namespace Client;

public class MessageReceivedEventArgs : EventArgs
{
    public required string Sender { get; set; }
    public required string Message { get; set; }
}