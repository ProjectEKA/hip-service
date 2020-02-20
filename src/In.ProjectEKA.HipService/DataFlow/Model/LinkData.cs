namespace In.ProjectEKA.HipService.DataFlow.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LinkData
    {
        [Key] public string LinkId { get; set; }
        public Entry Data { get; set; }
        public DateTime DateCreated { get; set; }
        public string Token { get; set; }

        public LinkData()
        {
        }

        public LinkData(string linkId, Entry data, DateTime dateCreated, string token)
        {
            LinkId = linkId;
            Data = data;
            DateCreated = dateCreated;
            Token = token;
        }
    }
}