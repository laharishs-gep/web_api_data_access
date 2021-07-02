using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_breach.Models
{
    public class UserAccessRights
    {
        [BsonId]
        public Guid Id { get; set; }
        public string UserRole { get; set; }
        public string CollectionName { get; set; }
        public string AccessString { get; set; }
    }
}