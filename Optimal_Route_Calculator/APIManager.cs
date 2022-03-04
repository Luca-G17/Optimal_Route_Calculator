using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
