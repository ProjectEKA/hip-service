namespace In.ProjectEKA.HipService.DataFlow.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HealthInformation
    {
        public HealthInformation()
        {
        }

        public HealthInformation(string informationId, Entry data, DateTime dateCreated, string token)
        {
            InformationId = informationId;
            Data = data;
            DateCreated = dateCreated;
            Token = token;
        }

        [Key]
        public string InformationId { get; set; }

        public Entry Data { get; set; }
        public DateTime DateCreated { get; set; }
        public string Token { get; set; }
    }
}