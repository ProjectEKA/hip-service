namespace In.ProjectEKA.HipService.Common
{
    using System.Collections.Generic;

    public class Token
    {
        public string ClientId { get; }
        public IEnumerable<string> Roles { get; }

        public Token(string clientId, IEnumerable<string> roles)
        {
            ClientId = clientId;
            Roles = roles;
        }
    }
}