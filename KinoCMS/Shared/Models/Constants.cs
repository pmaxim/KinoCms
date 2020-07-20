namespace KinoCMS.Shared.Models
{
    public class Constants
    {
        //public const string Domain = "https://e54b5058072e.ngrok.io";
#if DEBUG
        public const string Domain = "https://localhost:44312";
#else
        public const string Domain = "http://sms.serverpipe.com";
#endif

        public const string BaseUrl = "https://www.kinokopilka.pro/";
        public const int ImageSize = 200;
        public const string Base64 = "data:image/jpeg;base64,";
    }
}
