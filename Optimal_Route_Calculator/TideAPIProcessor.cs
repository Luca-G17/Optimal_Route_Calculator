using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Optimal_Route_Calculator
{
    class TideAPIProcessor
    {
        public static async Task<TideHeightModel> LoadTideData(string stationId)
        {
            // Sets the URL for the API with a longitude and latitude
            APIManager.ApiClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d4b511b338194272bd9080044c97cbe2");
            string url = $"https://admiraltyapi.azure-api.net/uktidalapi/api/V1/Stations/{stationId}/TidalEvents?";

            // Sends the GET request to the specified URL - Asynchronous
            using (HttpResponseMessage response = await APIManager.ApiClient.GetAsync(url))
            {
                // Checks if the request was sucessful before trying to access data
                if (response.IsSuccessStatusCode)
                {
                    //string test = await  response.Content.ReadAsStringAsync();
                    // Assigns the response to an instance of the WindModel - Asynchronous 
                    string json = await response.Content.ReadAsStringAsync();
                    TideHeightModel tidedata = new TideHeightModel(json);
                    return tidedata;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
