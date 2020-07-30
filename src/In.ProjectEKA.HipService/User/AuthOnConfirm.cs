namespace In.ProjectEKA.HipService.User
{
    public class AuthOnConfirm
    {
        public string AccessToken { get; set; }

        public AuthOnConfirm(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}