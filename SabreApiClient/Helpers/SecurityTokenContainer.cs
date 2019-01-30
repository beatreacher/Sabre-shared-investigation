using System;

namespace SabreApiClient.Helpers
{
    public class SecurityTokenContainer
    {
        public string BinarySecurityToken { get; }

        public SecurityTokenContainer(string binarySecurityToken)
        {
            BinarySecurityToken = binarySecurityToken;
        }
    }
}
