namespace In.ProjectEKA.HipService.Link.Model
{
    public class Auth
    {
        public Auth(string transactionId, Meta meta,Mode mode)
        {
            TransactionId = transactionId;
         //   AuthType = authType;
            Meta = meta;
            Mode = mode;
        }

        public string TransactionId { get; }
       // public AuthType AuthType { get; }
        public Meta Meta { get; }
        public Mode Mode { get; }
    }
}