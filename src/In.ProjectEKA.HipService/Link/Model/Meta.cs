namespace In.ProjectEKA.HipService.Link.Model
{
    using System;

    public class Meta
    {
        public Mode Mode { get; }
        public string Hint { get; }
        public DateTime Expiry { get; }
        
        public Meta(Mode mode, string hint, DateTime expiry)
        {
            Mode = mode;
            Hint = hint;
            Expiry = expiry;
        }

    }
}