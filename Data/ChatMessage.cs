namespace Data
{
    /// <summary>
    /// A single chat message for various purposes
    /// </summary>
    public class ChatMessage
    {
        public required string Sender { get; set; }
        public required string Content { get; set; }
    }
}