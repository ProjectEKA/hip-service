namespace In.ProjectEKA.HipService.Common.Model
{
    public class ConsentPurpose
    {
        public string Text { get; }
        public string Code { get; }
        public string RefUri { get; }

        public ConsentPurpose(string text, string code, string refUri)
        {
            Text = text;
            Code = code;
            RefUri = refUri;
        }
    }
}