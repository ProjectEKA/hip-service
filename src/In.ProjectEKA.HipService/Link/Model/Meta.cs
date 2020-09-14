namespace In.ProjectEKA.HipService.Link.Model
{
    using System;

    public class Meta
    {
        public Meta(string hint, DateTime expiry)
        {
            Hint = hint;
            Expiry = expiry;
        }
        
        public string Hint { get; }
        public DateTime Expiry { get; }
    }
}