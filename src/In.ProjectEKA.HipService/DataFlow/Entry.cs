namespace In.ProjectEKA.HipService.DataFlow
{
    public class Entry
    {
        public string Content { get; }

        public string Media { get; }

        public string Checksum { get; }

        public Entry(string content, string media, string checksum)
        {
            Content = content;
            Media = media;
            Checksum = checksum;
        }
    }
}