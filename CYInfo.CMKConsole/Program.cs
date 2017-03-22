using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CYInfo.CMKConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Call_GetShoes_Api();

            Call_GetFootSize_Api();
        }





        public static void Call_GetShoes_Api()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:4767/");//正式环境
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("X-ApiKey", "MyRandomApiKey:MyRandomApiKeyValue");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var authByteArray = Encoding.ASCII.GetBytes("AuthnticatedApiUser:PasswordForApi");
                var authString = Convert.ToBase64String(authByteArray);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                var content = new StringContent("{'CMD':'TSA001','PARAMS':{'A':1}}", Encoding.UTF8, "application/json");

                var message = client.PostAsync("api/GetShoes", content).Result.Content.ReadAsStringAsync().Result;

                string ttt = message;
            }
        }


        public static void Call_GetFootSize_Api()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:4767/");//正式环境
                client.Timeout = TimeSpan.FromMinutes(30);
                client.DefaultRequestHeaders.Add("X-ApiKey", "MyRandomApiKey:MyRandomApiKeyValue");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var authByteArray = Encoding.ASCII.GetBytes("AuthnticatedApiUser:PasswordForApi");
                var authString = Convert.ToBase64String(authByteArray);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

                var content = new StringContent("{'CMD':'TSA001','PARAMS':{'A':1}}", Encoding.UTF8, "application/json");

                var message = client.PostAsync("api/GetFootSize", content).Result.Content.ReadAsStringAsync().Result;

                string ttt = message;
            }
        }

    }
}
