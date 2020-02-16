using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;

namespace Ledom.Workshops.FooBakeryWebAPI
{
    public class MongoBakeryDataSource
    {
        static MongoBakeryDataSource()
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?), new NullableSerializer<decimal>(
                new DecimalSerializer(BsonType.Decimal128)));
        }

        public MongoClient Client { get; private set; } = null;
        public IMongoDatabase Database => Client?.GetDatabase("foobakery");

        public bool Connected { get; private set; } = false;

        public bool Connect()
        {
            if (Connected) return true;
            try
            {
                Client = new MongoClient("mongodb://localhost:27017"); // For demo purposes
                Connected = true;
                return true;
            }
            catch (Exception)
            {
                Connected = false;
                return false;
            }
        }
    }
}
