using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYInfo.CMKConsole
{
    public class DefaultMongoDb
    {
        public MongoDatabase database;
        protected static MongoServer _server;
        protected static MongoServerSettings _serverSettings;
        public IMongoDatabase IDatabase;
        public DefaultMongoDb(string databaseName)
        {

            try
            {


                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                MongoClient client = new MongoClient(ConnectionString);
                IDatabase = client.GetDatabase(databaseName);

                //string MongoIP = ConnectionString.Substring(10, ConnectionString.IndexOf(":", 10) - 10);
                _serverSettings = new MongoServerSettings() { };
                //_serverSettings.Server = new MongoServerAddress(MongoIP, 27017);
                _serverSettings.Servers = IDatabase.Client.Settings.Servers;
                _serverSettings.ReadPreference = IDatabase.Client.Settings.ReadPreference;


                //最大连接池数量
                _serverSettings.MaxConnectionPoolSize = 500;
                //等待列队数量
                _serverSettings.WaitQueueSize = 10;
                MongoDatabaseSettings _databaseSettings = new MongoDatabaseSettings();
                _server = new MongoServer(_serverSettings);
                database = new MongoDatabase(_server, databaseName, _databaseSettings);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
