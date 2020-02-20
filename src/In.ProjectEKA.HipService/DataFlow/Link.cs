namespace In.ProjectEKA.HipService.DataFlow
{
    public class Link
    {
        public string Href { get; set; }
        public string Media { get; set; }

        public Link(string href, string media)
        {
            Href = href;
            Media = media;
        }
    }
}