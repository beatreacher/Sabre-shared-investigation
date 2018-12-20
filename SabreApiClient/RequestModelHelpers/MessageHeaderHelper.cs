using Domain.Models;
using SabreApiClient.SessionCreator;
using System;

namespace SabreApiClient.RequestModelHelpers
{
    internal class MessageHeaderHelper
    {
        public static MessageHeader CreateMessageHeader(string conversationId, string actionName, string organization)
        {
            var tstamp = DateTime.UtcNow.ToString("s") + "Z";
            var msgHeader = new MessageHeader { ConversationId = conversationId };

            var from = new From
            {
                PartyId = new PartyId[1]
            };
            from.PartyId[0] = new PartyId { Value = "WebServiceClient" };
            msgHeader.From = from;

            var to = new To
            {
                PartyId = new PartyId[1]
            };
            to.PartyId[0] = new PartyId { Value = "WebServiceSupplier" };
            msgHeader.To = to;

            msgHeader.CPAId = organization;
            msgHeader.Action = actionName;

            var service = new Service { Value = actionName };
            msgHeader.Service = service;

            var msgData = new MessageData();
            msgData.MessageId = Guid.NewGuid().ToString();
            msgData.Timestamp = tstamp;
            msgHeader.MessageData = msgData;

            return msgHeader;
        }

        public static Security CreateSecurity(Session session)
        {
            return new Security { BinarySecurityToken = session.Token };
        }
    }
}
