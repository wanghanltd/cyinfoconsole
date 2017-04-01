using HtmlAgilityPack;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
        private static DefaultMongoDb DB = new DefaultMongoDb("BasicData");
        static void Main(string[] args)
        {
            //Call_GetShoes_Api();

            //Call_GetFootSize_Api();
            //GetBrandSizesUrl();
            //GetBrandSizes();
            //GetBrandSizesSpecial();
            //DataCleaning();
            //GetPrefix4Brand();

            CleanDataSizes4Brand();
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


                BsonDocument bosonEntity;
                ObjectId item_id;
                string url, brandName;

            foreach(var entity in findclasses)
            {
                
                url = entity.GetAttributeValue("href", "");
                brandName = entity.InnerHtml;

                bosonEntity = new BsonDocument();
                item_id = ObjectId.GenerateNewId();
                bosonEntity.Add("_id", item_id);
                bosonEntity.Add("BrandName", brandName);
                bosonEntity.Add("BrandSizeUrl", url);
                bosonEntity.Add("Status", 0);
                bosonEntity.Add("Created", DateTime.Now);
                SaveData2DB("Urls4Brand", bosonEntity);
            }

           return resultReturn;
        }



        public static string GetBrandSizes()
        {
            string resultReturn = string.Empty;

            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("utf-8");

            var targetCollection = DB.database.GetCollection("Urls4Brand");

      

            List<IMongoQuery> qryList = new List<IMongoQuery>();


            qryList.Add(Query.EQ("Status", 0));

            IMongoQuery query = Query.And(qryList);

            var entities = targetCollection.Find(query);

            foreach (var entity in entities)
            {

                try
                {


                    string brandName = entity["BrandName"].ToString();//"Adidas";
                    string brandSizeUrl = entity["BrandSizeUrl"].ToString();//"http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/adidas-size-chart/";

                    resultReturn = Encoding.UTF8.GetString(client.DownloadData(brandSizeUrl));

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
                    BsonDocument bosonEntity = new BsonDocument();
                    ObjectId item_id = ObjectId.GenerateNewId();
                    bosonEntity.Add("_id", item_id);
                    bosonEntity.Add("BrandName", brandName);
                    bosonEntity.Add("Status", 0);
                    bosonEntity.Add("Created", DateTime.Now);
                    foreach (var findclass in findclasses)
                    {
                        //Gender 1 男鞋；2 女性；3 儿童；4 婴儿
                        switch (i++)
                        {
                            case 0:
                                //women
                                bosonEntity.Add("Women", findclass.OuterHtml);
                                break; ;
                            case 1:
                                //men
                                bosonEntity.Add("Men", findclass.OuterHtml);
                                break;

                            case 2:
                                //kids
                                bosonEntity.Add("Kids", findclass.OuterHtml);
                                break;
                            case 3:
                                //Baby 
                                bosonEntity.Add("Baby", findclass.OuterHtml);
                                break;
                        }

                    }
                    SaveData2DB("Sizes4Brand", bosonEntity);

                    entity["Status"] = 1;
                }
                catch (Exception ex)
                {
                    entity["Status"] = 2;
                }

                targetCollection.Save(entity);
            }
            Console.ReadKey();
            return resultReturn;
        }



        public static string GetBrandSizesSpecial()
        {
            string resultReturn = string.Empty;

            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.GetEncoding("utf-8");

            var targetCollection = DB.database.GetCollection("Urls4Brand");
            


            List<IMongoQuery> qryList = new List<IMongoQuery>();


            qryList.Add(Query.EQ("Status", 2));

            IMongoQuery query = Query.And(qryList);

            var entities = targetCollection.Find(query);

            foreach (var entity in entities)
            {

                try
                {


                    string brandName = entity["BrandName"].ToString();//"Adidas";
                    string brandSizeUrl = entity["BrandSizeUrl"].ToString();//"http://www.shoesizeconversionchart.net/shoe-size-charts-by-brand-name/adidas-size-chart/";

                    resultReturn = Encoding.UTF8.GetString(client.DownloadData(brandSizeUrl));

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
                    BsonDocument bosonEntity = new BsonDocument();
                    ObjectId item_id = ObjectId.GenerateNewId();
                    bosonEntity.Add("_id", item_id);
                    bosonEntity.Add("BrandName", brandName);
                    bosonEntity.Add("Status", 0);
                    bosonEntity.Add("Created", DateTime.Now);
                    foreach (var findclass in findclasses)
                    {
                        //Gender 1 男鞋；2 女性；3 儿童；4 婴儿
                        switch (i++)
                        {
                            case 0:
                                //women
                                bosonEntity.Add("Women", findclass.OuterHtml);
                                break; ;
                            case 1:
                                //men
                                bosonEntity.Add("Men", findclass.OuterHtml);
                                break;

                            case 2:
                                //kids
                                bosonEntity.Add("Kids", findclass.OuterHtml);
                                break;
                            case 3:
                                //Baby 
                                bosonEntity.Add("Baby", findclass.OuterHtml);
                                break;
                        }

                    }
                    SaveData2DB("Sizes4Brand", bosonEntity);

                    entity["Status"] = 1;
                }
                catch (Exception ex)
                {
                    entity["Status"] = 2;
                }

                targetCollection.Save(entity);
            }
            Console.ReadKey();
            return resultReturn;
        }


        public static void DataCleaning()
        {
            try
            {
                var targetCollection = DB.database.GetCollection("Sizes4Brand");
                var entities = targetCollection.FindAll();
                HtmlDocument doc;
                string brandName;
                string[] genders = { "Women", "Men", "Kids", "Baby" };
                foreach (var entity in entities)
                {
                    brandName = entity["BrandName"].ToString();
                    foreach (string gender in genders)
                    {
                        if (entity.IndexOfName(gender) >= 0)
                        {
                            doc = new HtmlDocument();
                            doc.LoadHtml(entity[gender].ToString());
                            SaveSizeCharts(doc, brandName, gender);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveSizeCharts(HtmlDocument doc, string brandName, string gender)
        {

            
            BsonDocument bosonEntity;
            var findThs = doc.DocumentNode
                               .Descendants("th");

            List<string>  thsList = new List<string>();

            foreach (var findTh in findThs)
            {
                thsList.Add(findTh.InnerHtml);
            }

            string[] ths = thsList.ToArray();

            var findTrs = doc.DocumentNode
                       .Descendants("tr")
                       .Where(d =>
                            d.Attributes.Contains("class")
                            &&
                            !d.Attributes["class"].Value.Contains("row-1 odd")
                        );

            foreach (var findTr in findTrs)
            {
                bosonEntity = new BsonDocument();
                ObjectId item_id = ObjectId.GenerateNewId();
                bosonEntity.Add("_id", item_id);
                bosonEntity.Add("BrandName", brandName);
                bosonEntity.Add("Gender", gender);
                bosonEntity.Add("Created", DateTime.Now);
                bosonEntity.Add("Status", 0);
                int i = 0;
                foreach (var findTrTd in findTr.ChildNodes)
                {
                    bosonEntity.Add(ths[i++], findTrTd.InnerHtml);
                }
                SaveData2DB("Sizes4BrandReal", bosonEntity);
            }
        }



        public static void DataCleaningT()
        {
            try
            {
                var targetCollection = DB.database.GetCollection("Sizes4Brand");

                string brandName = "Adidas";
                string gender = "Women";

                List<IMongoQuery> qryList = new List<IMongoQuery>();
                qryList.Add(Query.EQ("BrandName", brandName));

                IMongoQuery query = Query.And(qryList);

                var entity = targetCollection.FindOne(query);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(entity[gender].ToString());

                var findThs = doc.DocumentNode
                           .Descendants("th");

                List<string> thsList = new List<string>();

                foreach (var findTh in findThs)
                {
                    thsList.Add(findTh.InnerHtml);
                }

                string[] ths = thsList.ToArray();

                var findTrs = doc.DocumentNode
                           .Descendants("tr")
                           .Where(d =>
                                d.Attributes.Contains("class")
                                &&
                                !d.Attributes["class"].Value.Contains("row-1 odd")
                            );

                BsonDocument bosonEntity = new BsonDocument();
                ObjectId item_id = ObjectId.GenerateNewId();
                bosonEntity.Add("_id", item_id);
                bosonEntity.Add("BrandName", brandName);
                bosonEntity.Add("Gender", gender);
                bosonEntity.Add("Status", 0);
                bosonEntity.Add("Created", DateTime.Now); ;

                BsonArray chartArr = new BsonArray();
                BsonDocument charDic;
                foreach (var findTr in findTrs)
                {
                    charDic = new BsonDocument();
                    int i = 0;
                    foreach (var findTrTd in findTr.ChildNodes)
                    {
                        charDic.Add(ths[i++], findTrTd.InnerHtml);
                    }
                    chartArr.Add(charDic);
                }
                bosonEntity.Add("SizeCharts", chartArr);
                SaveData2DB("Sizes4BrandReal", bosonEntity);




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }

        public static void SaveData2DB(string collectionName, BsonDocument bsonEntity)
        {
            var targetCollection = DB.database.GetCollection(collectionName);
                targetCollection.Save(bsonEntity);
        }


        public static void GetPrefix4Brand()
        {

            var targetCollection = DB.database.GetCollection("Urls4Brand");

            
            List<IMongoQuery> qryList = new List<IMongoQuery>();


            qryList.Add(Query.EQ("Status", 2));

            IMongoQuery query = Query.And(qryList);

            var entities = targetCollection.Find(query);
            BsonDocument bosonEntity = new BsonDocument();
            ObjectId item_id;
            string brandName = string.Empty,brandPrefix=string.Empty;
            foreach (var entity in entities)
            {
                try
                {

                   
                    brandName = entity["BrandName"].ToString();
                    brandPrefix = brandName.Substring(0, 1);
                    CollectingBrandName(brandPrefix,brandName);
                    entity["Status"] = 2;
                }
                catch (Exception ex)
                {
                    entity["Status"] = 11;
                }
                targetCollection.Save(entity);
            }
        }


        public static void CollectingBrandName(string brandPrefix,string brandName)
        {

            var targetCollection = DB.database.GetCollection("Prefix4Brand");


            List<IMongoQuery> qryList = new List<IMongoQuery>();


            qryList.Add(Query.EQ("BrandPrefix", brandPrefix));

            IMongoQuery query = Query.And(qryList);

            var entity = targetCollection.FindOne(query);
            if(entity==null)
            {
                BsonDocument bosonEntity = new BsonDocument();
                ObjectId item_id = ObjectId.GenerateNewId(); ;
                bosonEntity.Add("BrandPrefix", brandPrefix);
                BsonArray brandsArray = new BsonArray();

                BsonDocument brandEntity = new BsonDocument();
                brandEntity.Add("BrandName", brandName);
                //brandEntity.Add("Created", DateTime.Now);
                brandsArray.Add(brandEntity);

                bosonEntity.Add("Brands", brandsArray);

                bosonEntity.Add("Status", 0);
                bosonEntity.Add("Created", DateTime.Now);
                targetCollection.Save(bosonEntity);

            }
            else
            {
                BsonDocument brandEntity = new BsonDocument();
                brandEntity.Add("BrandName", brandName);
                //brandEntity.Add("Created", DateTime.Now);
                entity["Brands"].AsBsonArray.Add(brandEntity);
                targetCollection.Save(entity);
            }

        }


        public static void CleanDataSizes4Brand()
        {


            try
            {
                var targetCollection = DB.database.GetCollection("Sizes4BrandPure");



                BsonDocument query = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>("{\"Women\": { $not: /.*Women's.*/i },\"Men\": { $exists: false } }");
                QueryDocument queryDoc = new QueryDocument(query);

                var entities = targetCollection.Find(queryDoc);
                string[] genders = { "Men's", "Kids's", "Baby's" };
                foreach(var entity in entities)
                {

                    string charts = entity["Women"].ToString();
                   
                        if (charts.ToLower().Contains("men's"))
                        {

                            entity["Men"] = charts;
                            entity["Women"] = string.Empty;
                            
                        }
                        else if (charts.ToLower().Contains("kids's"))
                        {
                            entity["Kids"] = charts;
                            entity["Women"] = string.Empty;
                        }
                        else if (charts.ToLower().Contains("baby's"))
                        {
                            entity["Baby"] = charts;
                            entity["Women"] = string.Empty;
                        }
              
                    
                    targetCollection.Save(entity);
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


    }
}
