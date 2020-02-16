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
    [Route("/customer")]
    public class CustomersController : ControllerBase
    {
        private readonly MongoBakeryDataSource _dataSource;

        public CustomersController(MongoBakeryDataSource dataSource)
        {
            _dataSource = dataSource;
            _dataSource.Connect();
        }

        // GET /customer/{id}
        [HttpGet("{id}")]
        public CustomerDTO GetCustomer([FromRoute]int id)
        {
            // Build a search filter to select all customers with "id" set to the requested id.
            var filter = Builders<BsonDocument>.Filter.Eq("ID", id);
            // Perform the actual search
            BsonDocument customerBson = _dataSource.Database
                            .GetCollection<BsonDocument>("customers")
                            .Find(filter) // Use the above filter
                            .FirstOrDefault(); // Will return null if no customer is found.
            if (customerBson != null)
                return new CustomerDTO(BsonSerializer.Deserialize<Customer>(customerBson)); // We have a valid customer to return.
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null; // We have found no customer with the given id.
            }
        }

        // GET /customer/all
        [HttpGet("all")]
        public CustomerDTO[] GetAllCustomers()
        {
            // The following filter matches everything, thus resulting in all documents being returned.
            var filter = Builders<BsonDocument>.Filter.Empty;
            // Perform the actual search
            var results = _dataSource.Database
                            .GetCollection<BsonDocument>("customers")
                            .Find(filter) // Use the above filter
                            .ToEnumerable(); // Get a nice enumerable collection
            // Finally create customer entities and pack them into an array.
            return results
                    .Select((b) => new CustomerDTO(BsonSerializer.Deserialize<Customer>(b)))
                    .ToArray();
        }

        // POST /customer
        [HttpPost]
        public void PostCustomer([FromBody] CustomerDTO customer)
        {
            if (customer != null)
            {
                try
                {
                    _dataSource.Database
                        .GetCollection<BsonDocument>("customers")
                        .InsertOne(customer.ToEntity(_idValue: new ObjectId()).ToBsonDocument());
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
