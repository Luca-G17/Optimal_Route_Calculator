using System.Net.Http;
using System.Net.Http.Headers;

namespace Optimal_Route_Calculator
{
    static class APIManager
    {
        public static HttpClient ApiClient { get; set; }
        public static void InitialiseClient()
        {
            // Sets up the Http Client and clears existing headers
            ApiClient = new HttpClient();
            ResetClient();
        }
        public static void ResetClient()
        {
            // Clears existing headers and adds JSON as a media return type
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
