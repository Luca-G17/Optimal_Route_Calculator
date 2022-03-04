<<<<<<< HEAD
﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Optimal_Route_Calculator
{
    class WindAPIProcessor
    {
        public static async Task<WindModel> LoadWindData(double[] location)
        {
            // Sets the URL for the API with a longitude and latitude
            string url = $"https://api.darksky.net/forecast/f9d408baa1fdc7017398cbd27acbee7d/{ location[0] }, { location[1] }";

            // Sends the GET request to the specified URL - Asynchronous
            using (HttpResponseMessage response = await APIManager.ApiClient.GetAsync(url))
            {
                // Checks if the request was sucessful before trying to access data
                if (response.IsSuccessStatusCode)
                {
                    // Assigns the response to an instance of the WindModel - Asynchronous 
                    WindModel winddata = await response.Content.ReadAsAsync<WindModel>();
                    winddata.SetWindData();
                    return winddata;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
=======
﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Optimal_Route_Calculator
{
    class WindAPIProcessor
    {
        public static async Task<WindModel> LoadWindData(double[] location)
        {
            string url = $"https://api.darksky.net/forecast/f9d408baa1fdc7017398cbd27acbee7d/{ location[0] }, { location[1] }" ;

            using (HttpResponseMessage response = await APIManager.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    WindModel winddata = await response.Content.ReadAsAsync<WindModel>();
                    winddata.setWindData();
                    return winddata;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
>>>>>>> 3da34f7792296f9183bb3aefb50d77191f829b09
