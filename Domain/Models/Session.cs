namespace Domain.Models
{
    public class Session
    {
        public string Token { get; }

        public string ConversationId { get; }

        public string MessageId { get; }

        public string TimeStamp { get; }

        public Session(string token, string conversationId, string messageId, string timeStamp)
        {
            Token = token;
            ConversationId = conversationId;
            MessageId = messageId;
            TimeStamp = timeStamp;
        }
    }
}
