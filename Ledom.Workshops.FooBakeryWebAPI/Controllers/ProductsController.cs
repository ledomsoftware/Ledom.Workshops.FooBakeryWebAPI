using Ledom.Workshops.FooBakeryWebAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Net;

namespace Ledom.Workshops.FooBakeryWebAPI.Controllers
{
    [ApiController]
    [Route("/product")]
    public class ProductsController : ControllerBase
    {
        private readonly MongoBakeryDataSource _dataSource;

        public ProductsController(MongoBakeryDataSource dataSource)
        {
            _dataSource = dataSource;
            _dataSource.Connect();
        }

        // GET /product/{id}
        [HttpGet("{id}")]
        public ProductDTO GetProduct([FromRoute]int id)
        {
            // Build a search filter to select all products with "id" set to the requested id.
            var filter = Builders<BsonDocument>.Filter.Eq("ID", id);
            // Perform the actual search
            BsonDocument productBson = _dataSource.Database
                            .GetCollection<BsonDocument>("products")
                            .Find(filter) // Use the above filter
                            .FirstOrDefault(); // Will return null if no product is found.
            if (productBson != null)
                return new ProductDTO(BsonSerializer.Deserialize<Product>(productBson)); // We have a valid product to return.
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null; // We have found no product with the given id.
            }
        }

        // GET /product/all
        [HttpGet("all")]
        public ProductDTO[] GetAllProducts()
        {
            // The following filter matches everything, thus resulting in all documents being returned.
            var filter = Builders<BsonDocument>.Filter.Empty;
            // Perform the actual search
            var results = _dataSource.Database
                            .GetCollection<BsonDocument>("products")
                            .Find(filter) // Use the above filter
                            .ToEnumerable(); // Get a nice enumerable collection
            // Finally create product entities and pack them into an array.
            return results.Select((b) => new ProductDTO(BsonSerializer.Deserialize<Product>(b))).ToArray();
        }

        // POST /product
        [HttpPost]
        public void PostProduct([FromBody]ProductDTO product)
        {
            if (product != null)
            {
                try
                {
                    _dataSource.Database
                        .GetCollection<BsonDocument>("products")
                        .InsertOne(product.ToEntity(_idValue: new ObjectId()).ToBsonDocument());
                    Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (Exception)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}
