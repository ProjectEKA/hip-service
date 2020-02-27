namespace In.ProjectEKA.HipService.DataFlow
{
    public class Entry
    {
        public string Content { get; set; }

        public string Media { get; set; }

        public string Checksum { get; set; }
        public Link Link { get; set; }

        public Entry(string content, string media, string checksum, Link link)
        {
            Content = content;
            Media = media;
            Checksum = checksum;
            Link = link;
        }
    }
}