using In.ProjectEKA.HipService.Link.Model;

namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public AuthOnConfirm(string accessToken,Patient patient)
        {
            AccessToken = accessToken;
            Patient = patient;
        }

        public string AccessToken { get; set; }
        public Patient Patient { get; set; }
    }
}