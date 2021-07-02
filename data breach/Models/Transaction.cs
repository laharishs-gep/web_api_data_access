using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_breach.Models
{
    public class Transaction
    {
        public class TransactionRecord
        {
            [BsonId]
            public Guid Id { get; set; }
            public InformationCluster_1 InformationCluster_1 { get; set; }
            public InformationCluster_2 InformationCluster_2 { get; set; }
            public InformationCluster_3 InformationCluster_3 { get; set; }
        }


        public class InformationCluster_1
        {
            public string InformationID_1 { get; set; }
            public string InformationID_2 { get; set; }
            public string InformationID_3 { get; set; }
        }


        public class InformationCluster_2
        {
            public string InformationID_4 { get; set; }
            public string InformationID_5 { get; set; }
            public string InformationID_6 { get; set; }
        }


        public class InformationCluster_3
        {
            public string InformationID_7 { get; set; }
            public string InformationID_8 { get; set; }
            public string InformationID_9 { get; set; }
        }
    }
}
