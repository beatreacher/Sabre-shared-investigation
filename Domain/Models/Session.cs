namespace Domain.Models
{
    public class Session
    {
        public string Token { get; }

        public string ConversationId { get; }

        public string MessageId { get; }

        public string TimeStamp { get; }

        public string Organization { get; }

        public Session(string token, string conversationId, string messageId, string timeStamp, string organization)
        {
            Token = token;
            ConversationId = conversationId;
            MessageId = messageId;
            TimeStamp = timeStamp;
            Organization = organization;
        }
    }
}
