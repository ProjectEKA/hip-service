namespace In.ProjectEKA.HipService.DataFlow
{
    public class Entry
    {
        public Entry(string content, string media, string checksum, string link, string careContextReference)
        {
            Content = content;
            Media = media;
            Checksum = checksum;
            Link = link;
            CareContextReference = careContextReference;
        }

        public string Content { get; set; }

        public string Media { get; set; }

        public string CareContextReference { get; set; }
        public string Checksum { get; set; }
        public string Link { get; set; }
    }
}