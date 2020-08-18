namespace In.ProjectEKA.HipService.Link
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Communication
    {
        public Communication(CommunicationMode mode, string value)
        {
            Mode = mode;
            Value = value;
        }

        public CommunicationMode Mode { get; }

        public string Value { get; }
    }
}