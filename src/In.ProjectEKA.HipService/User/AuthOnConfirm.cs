namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public AuthOnConfirm(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; set; }
    }
}