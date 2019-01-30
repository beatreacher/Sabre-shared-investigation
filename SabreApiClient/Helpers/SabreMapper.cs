using AutoMapper;

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

        public TSecurity GetSecurity<TSecurity>(string token)
        {
            var securityTokenContainer = new SecurityTokenContainer(token);
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SecurityTokenContainer, TSecurity>(); });
            return config.CreateMapper().Map<TSecurity>(securityTokenContainer);
        }
    }
}
