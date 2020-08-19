namespace In.ProjectEKA.HipService.Common
{
    using System.Collections.Generic;

    public class Token
    {
        public Token(IEnumerable<string> roles)
        {
            Roles = roles;
        }

        public IEnumerable<string> Roles { get; }
    }
}