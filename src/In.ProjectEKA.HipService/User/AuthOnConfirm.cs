using In.ProjectEKA.HipLibrary.Patient.Model;
using System.Collections.Generic;
namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public AuthOnConfirm(string accessToken,Link.Model.Patient patient,List<Identifier> identifier)
        {
            AccessToken = accessToken;
            Patient = patient;
            Identifier = identifier;
        }

        public string AccessToken { get; set; }
        public Link.Model.Patient Patient { get; set; }
        public List<Identifier> Identifier { get; set; }
    }
}