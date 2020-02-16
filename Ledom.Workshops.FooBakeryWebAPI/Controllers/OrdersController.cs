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
    [Route("/order")]
    public class OrdersController : ControllerBase
    {
        private readonly MongoBakeryDataSource _dataSource;

        public OrdersController(MongoBakeryDataSource dataSource)
        {
            _dataSource = dataSource;
            _dataSource.Connect();
        }

        // GET /order/{id}
        [HttpGet("{id}")]
        public OrderDTO GetOrder([FromRoute]int id)
        {
            // Build a search filter to select all orders with "ID" set to the requested id.
            var filter = Builders<BsonDocument>.Filter.Eq("ID", id);
            // Perform the actual search
            BsonDocument orderBson = _dataSource.Database
                            .GetCollection<BsonDocument>("orders")
                            .Find(filter) // Use the above filter
                            .FirstOrDefault(); // Will return null if no order is found.
            if (orderBson != null)
                return new OrderDTO(BsonSerializer.Deserialize<Order>(orderBson)); // We have a valid order to return.
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null; // We have found no order with the given id.
            }
        }

        // GET /order/all
        [HttpGet("all")]
        public OrderDTO[] GetAllOrders()
        {
            // The following filter matches everything, thus resulting in all documents being returned.
            var filter = Builders<BsonDocument>.Filter.Empty;
            // Perform the actual search
            var results = _dataSource.Database
                            .GetCollection<BsonDocument>("orders")
                            .Find(filter) // Use the above filter
                            .ToEnumerable(); // Get a nice enumerable collection
            // Finally create order entities and pack them into an array.
            return results
                    .Select((b) => new OrderDTO(BsonSerializer.Deserialize<Order>(b)))
                    .ToArray();
        }

        // POST /order
        [HttpPost]
        public void PostOrder([FromBody]OrderDTO order)
        {
            if (order != null)
            {
                try
                {
                    _dataSource.Database
                        .GetCollection<BsonDocument>("orders")
                        .InsertOne(order.ToEntity(_idValue: new ObjectId()).ToBsonDocument());
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
