namespace In.ProjectEKA.TMHHip.DataFlow
{
    using System;
    using System.Net;
    using System.Security;

    internal class CustomWebclient : WebClient
    {
        [SecuritySafeCritical]
        private CustomWebclient() : base()
        {
        }

        private readonly CookieContainer cookieContainer = new CookieContainer();


        protected override WebRequest GetWebRequest(Uri myAddress)
        {
            WebRequest request = base.GetWebRequest(myAddress);
            if (!(request is HttpWebRequest)) return request;
            (request as HttpWebRequest).CookieContainer = cookieContainer;
            (request as HttpWebRequest).AllowAutoRedirect = true;
            return request;
        }


        public static string PdfToBase64(string pdfUrl)
        {
            using var webClient = new CustomWebclient();
            var bytes = webClient.DownloadData(pdfUrl);
            return Convert.ToBase64String(bytes);
        }
    }
}