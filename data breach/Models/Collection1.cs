using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_breach.Models
{
    public class Collection1
    {
        public ObjectId Id { get; set; }

        public string name { get; set; }

        public string author { get; set; }

        public string user { get; set; }
    }
}
