using data_breach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace data_breach.Services
{
    public class RecordService
    {
        private readonly IMongoCollection<Collection1> _coll1;

        public RecordService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _coll1 = database.GetCollection<Collection1>("Collection1");
        }

        public List<Collection1> Get() =>
            _coll1.Find(coll => true).ToList();

        public Collection1 Create(Collection1 coll)
        {
            _coll1.InsertOne(coll);
            return coll;
        }

        public void Update(string name, Collection1 collIn) => 
            _coll1.ReplaceOne(coll => coll.name == name, collIn);
    }
}
