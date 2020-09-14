namespace In.ProjectEKA.HipService.Link.Model
{
    public class Auth
    {
        public Auth(string transactionId, Meta meta,Mode mode)
        {
            TransactionId = transactionId;
            Meta = meta;
            Mode = mode;
        }

        public string TransactionId { get; }
        public Meta Meta { get; }
        public Mode Mode { get; }
    }
}