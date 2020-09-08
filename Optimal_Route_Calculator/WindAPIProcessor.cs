using System;
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
