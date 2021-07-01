using data_breach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

namespace data_breach.Services
{
    public class RecordService
    {
        protected static IMongoDatabase _database;
        private readonly IMongoCollection<Collection1> _coll1;
        private readonly IMongoCollection<User> _users;

        private readonly IMongoClient _client;

        public RecordService(IDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);

            _coll1 = _database.GetCollection<Collection1>(settings.Collection);
            _users = _database.GetCollection<User>("users");
        }

        

        public List<BsonDocument> Get(bool getusers)
        {
            var users = _users.Find(FilterDefinition<User>.Empty)
                .Project(Builders<User>.Projection.Include("user").Exclude("_id"));
            return users.ToList();
        }

        public List<string> Get()
        {
            return _database.ListCollectionNames().ToList();
        }

        public string Get(string name, string collName)
        {
            return _coll1.Find(x => x.name == name).ToString();
        }

        public Collection1 Create(Collection1 coll)
        {
            _coll1.InsertOne(coll);
            return coll;
        }

        public void Update(string name, Collection1 collIn) => 
            _coll1.ReplaceOne(coll => coll.name == name, collIn);

        public void Update(string collName, string role, User new_user)
        {
            _users.ReplaceOne(user => user.user == role, new_user);

        }
    }
}
