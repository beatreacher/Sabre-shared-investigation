namespace Domain.Models
{
    public class Session
    {
        public string Token { get; private set; }

        public string ConversationId { get; private set; }

        public string MessageId { get; private set; }

        public string TimeStamp { get; private set; }

        public Session(string token, string conversationId, string messageId, string timeStamp)
        {
            Token = token;
            ConversationId = conversationId;
            MessageId = messageId;
            TimeStamp = timeStamp;
        }
    }
}
