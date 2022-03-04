<<<<<<< HEAD
﻿using System.Net.Http;
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
=======
﻿using System;
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
            // Opens the TCP/IP port and clears existing headers
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
>>>>>>> 3da34f7792296f9183bb3aefb50d77191f829b09
