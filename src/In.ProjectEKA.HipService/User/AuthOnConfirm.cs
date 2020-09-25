using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public AuthOnConfirm(string accessToken,Link.Model.Patient patient,Identifier identifier)
        {
            AccessToken = accessToken;
            Patient = patient;
            Identifier = identifier;
        }

        public string AccessToken { get; set; }
        public Link.Model.Patient Patient { get; set; }
        public Identifier Identifier { get; set; }
    }
}