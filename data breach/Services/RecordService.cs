using data_breach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using System.Text.Json;

namespace data_breach.Services
{
    public class RecordService
    {
        protected static IMongoDatabase _database;
        private readonly IMongoCollection<Transaction> _transactions;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserAccessRights> _accessRights;

        private readonly IMongoClient _client;

        public RecordService(IDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);

            _transactions = _database.GetCollection<Transaction>("Transactions");
            _users = _database.GetCollection<User>("Users");
            _accessRights = _database.GetCollection<UserAccessRights>("AccessRights");
        }

        public List<UserAccessRights> GetRoles()
        {
            var fieldsBuilder = Builders<UserAccessRights>.Projection;
            var fields = fieldsBuilder.Exclude(d => d.AccessString).Exclude(d => d.CollectionName);

            var result = _accessRights.Find(x => true).Project<UserAccessRights>(fields).ToList();

            return result;
        }

        public List<string> GetCollections()
        {
            return _database.ListCollectionNames().ToList();
        }

        public void InsertDocument(string collectionName, string document)
        {
            BsonDocument bsonDocument = BsonDocument.Parse(document);
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(bsonDocument);
        }

        public List<object> LoadDocuments(string collectionName, string userId = null)
        {
            if (userId == null)
            {
                userId = "ed9f3e47-861c-403d-8d32-776bfd608936";
            }
           
            Guid userGuid = new Guid(userId);
            string userAccessString = GetAccessString(GetUserRole(userGuid), "Transactions");
            var collection = _database.GetCollection<Transaction>("Transactions");
            userAccessString = GenerateProjectionString(userAccessString);
            if (userAccessString == "")
            {
                return null;
            }

            var result = collection.Find(new BsonDocument()).Project(userAccessString).ToList();
            var res_final = result.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            return res_final;
        }

        public void UpdateAccess(string userRole, string collectionName, string newAccessString)
        {
            var collection = _database.GetCollection<UserAccessRights>("AccessRights");
            var builder = Builders<UserAccessRights>.Filter;
            var filter = builder.Eq("UserRole", userRole) & builder.Eq("CollectionName", collectionName);
            UserAccessRights UserAccessRightInstance = collection.Find(filter).First();
            UserAccessRightInstance.AccessString = newAccessString;
            var result = collection.ReplaceOne
                (
                    new BsonDocument("UserRole", userRole),
                    UserAccessRightInstance,
                    new ReplaceOptions { IsUpsert = true }
                );
        }

        public string ToGUID(string hex)
        {
            var a = hex.Substring(6, 2) + hex.Substring(4, 2) + hex.Substring(2, 2) + hex.Substring(0, 2);
            var b = hex.Substring(10, 2) + hex.Substring(8, 2);
            var c = hex.Substring(14, 2) + hex.Substring(12, 2);
            var d = hex.Substring(16, 16);
            hex = a + b + c + d;
            var uuid = hex.Substring(0, 8) + '-' + hex.Substring(8, 4) + '-' + hex.Substring(12, 4) + '-' + hex.Substring(16, 4) + '-' + hex.Substring(20, 12);
            return '"' + uuid + '"';
        }

        public string GenerateProjectionString(string userAccessString)
        {
            int[] startingIndexArray = new int[100];
            int[] endingIndexArray = new int[100];
            int counter = 0;

            userAccessString = userAccessString.Replace(" ", "");

            for (int i = userAccessString.Length - 1; i >= 0; i--)
            {
                if (userAccessString[i] == '0' && userAccessString[i - 1] == ':')
                {
                    endingIndexArray[counter] = i;
                    while (userAccessString[i] != '{' && userAccessString[i] != ',')
                    {
                        i--;
                    }
                    if (userAccessString[i] == ',')
                    {
                        i--;
                    }
                    else
                    {
                        endingIndexArray[counter] += 1;
                    }
                    startingIndexArray[counter] = i + 1;
                    counter++;
                    i++;
                }
            }



            for (int c = 0; c < counter; c++)
            {
                int n = endingIndexArray[c] - startingIndexArray[c] + 1;
                if ((c > 0) && (endingIndexArray[c] == startingIndexArray[c - 1]))
                {
                    n--;
                }
                userAccessString = userAccessString.Remove(startingIndexArray[c], n);
            }

            userAccessString = userAccessString.Replace("{,", "{");

            startingIndexArray = new int[100];
            endingIndexArray = new int[100];
            counter = 0;

            while (true)
            {
                if (!userAccessString.Contains("{}"))
                {
                    break;
                }
                if (userAccessString == "{}")
                {
                    userAccessString = "";
                    break;
                }
                for (int i = userAccessString.Length - 1; i >= 0; i--)
                {
                    if (userAccessString[i] == '}' && userAccessString[i - 1] == '{')
                    {
                        endingIndexArray[counter] = i;
                        i -= 2;
                        while (userAccessString[i] != '{' && userAccessString[i] != ',')
                        {
                            i--;
                        }
                        if (userAccessString[i] == ',')
                        {
                            i--;
                        }
                        else
                        {
                            endingIndexArray[counter] += 1;
                        }
                        startingIndexArray[counter] = i + 1;
                        counter++;
                        i++;
                    }
                }
                for (int c = 0; c < counter; c++)
                {
                    int n = endingIndexArray[c] - startingIndexArray[c] + 1;
                    if ((c > 0) && (endingIndexArray[c] == startingIndexArray[c - 1]))
                    {
                        n--;
                    }
                    userAccessString = userAccessString.Remove(startingIndexArray[c], n);
                }
            }

            userAccessString = userAccessString.Replace("{,", "{");
            return userAccessString;
        }

        public string GetUserRole(Guid userId)
        {
            var collection = _database.GetCollection<User>("Users");
            var filter = Builders<User>.Filter.Eq("Id", userId);
            string userRole = collection.Find(filter).First().UserRole;
            return userRole;
        }

        public string GetAccessString(string userRole, string collectionName)
        {
            var collection = _database.GetCollection<UserAccessRights>("AccessRights");
            var builder = Builders<UserAccessRights>.Filter;
            var filter = builder.Eq("UserRole", userRole) & builder.Eq("CollectionName", collectionName);
            string accessString = collection.Find(filter).First().AccessString;
            return accessString;
        }


        public void UpdateAccessString(string userRole, string collectionName, string newAccessString)
        {
            var collection = _database.GetCollection<UserAccessRights>("AccessRights");
            var builder = Builders<UserAccessRights>.Filter;
            var filter = builder.Eq("UserRole", userRole) & builder.Eq("CollectionName", collectionName);
            UserAccessRights UserAccessRightInstance = collection.Find(filter).First();
            UserAccessRightInstance.AccessString = newAccessString;
            var result = collection.ReplaceOne
                (
                    new BsonDocument("UserRole", userRole),
                    UserAccessRightInstance,
                    new ReplaceOptions { IsUpsert = true }
                );
            Console.WriteLine(result);
        }

    }
}
