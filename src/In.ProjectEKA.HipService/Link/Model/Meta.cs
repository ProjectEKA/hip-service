namespace In.ProjectEKA.HipService.Link.Model
{
    using System;

    public class Meta
    {
        public Meta(Mode mode, string hint, DateTime expiry)
        {
            Mode = mode;
            Hint = hint;
            Expiry = expiry;
        }

        public Mode Mode { get; }
        public string Hint { get; }
        public DateTime Expiry { get; }
    }
}