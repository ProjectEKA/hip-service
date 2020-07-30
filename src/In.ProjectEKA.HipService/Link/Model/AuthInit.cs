namespace In.ProjectEKA.HipService.Link.Model
{
    using System;

    public class AuthInit
    {
        public string TransactionId { get; }
        public AuthType AuthType { get; }
        public Meta Meta { get; }
        
        public AuthInit(string transactionId, AuthType authType, Meta meta)
        {
            TransactionId = transactionId;
            AuthType = authType;
            Meta = meta;
        }
    }
}