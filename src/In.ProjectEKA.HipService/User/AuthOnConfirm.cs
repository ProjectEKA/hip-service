using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public AuthOnConfirm(string accessToken,Link.Model.Patient patient)
        {
            AccessToken = accessToken;
            Patient = patient;
        }

        public string AccessToken { get; set; }
        public Link.Model.Patient Patient { get; set; }
    }
}