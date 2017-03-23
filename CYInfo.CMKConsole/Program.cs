using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            //Call_GetFootSize_Api();

            GetBrandSizes();
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



        public static string GetBrandSizesUrl()
        {
            string resultReturn = string.Empty;

            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("utf-8");



            resultReturn = Encoding.UTF8.GetString(client.DownloadData("http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/"));

            StringBuilder pureText = new StringBuilder();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(resultReturn);

                var findclasses = doc.DocumentNode
                    .Descendants( "a" )
                    .Where( d => 
                        d.Attributes.Contains("href")
                        &&
                        d.Attributes["href"].Value.Contains("http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/")
                    );

            foreach(var entity in findclasses)
            {
                string url = entity.GetAttributeValue("href", "");
                string brandName = entity.InnerHtml;
            }

           return resultReturn;
        }



        public static string GetBrandSizes()
        {
            string resultReturn = string.Empty;

            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("utf-8");



            //resultReturn = Encoding.UTF8.GetString(client.DownloadData("http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/acorn-size-chart/"));
            resultReturn = Encoding.UTF8.GetString(client.DownloadData("http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/adidas-size-chart/"));
            
            StringBuilder pureText = new StringBuilder();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(resultReturn);

            var findclasses = doc.DocumentNode
                    .Descendants("div")
                    .Where(d =>
                        d.Attributes.Contains("class")
                        &&
                        d.Attributes["class"].Value.Contains("tablepress-scroll-wrapper")
                    );


            int i = 0;
            foreach (var entity in findclasses)
            {
               switch(i++)
               {
                   case 0:
                       //women
                       Console.WriteLine("women:");
                       Console.WriteLine(entity.OuterHtml);
                       continue;
                   case 1:
                       //men
                       Console.WriteLine("men:");
                       continue;

                   case 2:
                       //kids
                       Console.WriteLine("kids:");
                       continue;
                   case 3:
                       //Baby 
                       Console.WriteLine("Baby:");
                       continue;
               }

            }

            Console.ReadKey();
            return resultReturn;
        }


    }
}
