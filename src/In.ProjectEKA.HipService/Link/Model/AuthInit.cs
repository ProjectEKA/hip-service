namespace In.ProjectEKA.HipService.Link.Model
{
    public class AuthInit
    {
        public AuthInit(string transactionId, AuthType authType, Meta meta)
        {
            TransactionId = transactionId;
            AuthType = authType;
            Meta = meta;
        }

        public string TransactionId { get; }
        public AuthType AuthType { get; }
        public Meta Meta { get; }
    }
}