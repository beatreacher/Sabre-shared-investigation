namespace SabreApiClient.Helpers
{
    internal class SabreMapper
    {
        static  SabreMapper()
        {
            AutoMapper.Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
        }

        public T GetMessageHeader<T>(string conversationId, string actionName, string organization)
        {
            var msgHeader = MessageHeaderHelper.CreateMessageHeader(conversationId, actionName, organization);
            return AutoMapper.Mapper.Map<T>(msgHeader);
        }
    }
}
