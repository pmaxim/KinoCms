using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace KinoCMS.Server.Lib
{
    public static class HttpLib
    {
        public static async Task<string> Get(string url)
        {
            var isInternetConnected = NetworkInterface.GetIsNetworkAvailable();
            if (!isInternetConnected) return string.Empty;
            var requestUri = new Uri(url);
            using var handler = new HttpClientHandler();
            using var httpClient = new HttpClient(handler) { BaseAddress = requestUri };

            string response;
            try
            {
                var httpResponse = await httpClient.GetAsync(requestUri);
                response = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                var d = e.Message;
                return string.Empty;
            }
            return response;
        }
    }
}
