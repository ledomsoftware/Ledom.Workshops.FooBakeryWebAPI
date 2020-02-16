using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ledom.Workshops.FooBakeryWebAPI.Entities
{
    public class OrderRecord
    {
        public int ProductID { get; set; } = 0;

        public decimal Quantity { get; set; } = 0;

        public OrderRecord()
        {
        }
    }

    /// <summary>
    /// Represents a <see cref="Customer"/>'s order
    /// </summary>
    public class Order : Entity
    {
        /// <summary>
        /// ID (primary key) of this product.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The date and time on wich this order was placed.
        /// </summary>
        public DateTime Placed { get; set; }

        /// <summary>
        /// The date and time on which this order was processed and fullfilled. Nullable.
        /// </summary>
        public DateTime? Fullfilled { get; set; } = null;

        /// <summary>
        /// The ID of the <see cref="Customer"/> who placed this order.
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// The ID and quantity of each ordered good.
        /// </summary>
        public IList<OrderRecord> ProductQuantities { get; set; }
            = new List<OrderRecord>();

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="placed">The date and time on wich this order was placed.</param>
        /// <param name="customer">The <see cref="Customer"/> who placed this order. Must not be null.</param>
        /// <param name="records">The ID and quantity of each ordered good. Leave null to create an empty order.</param>
        public Order(int id, DateTime placed, int customerId, IEnumerable<OrderRecord> records = null)
        {
            ID = id;
            Placed = placed;
            CustomerID = Math.Max(customerId, 0);
            if (records != null)
                ProductQuantities = new List<OrderRecord>(records);
            else
                ProductQuantities = new List<OrderRecord>();
        }

        /// <summary>
        /// Used while deserializing
        /// </summary>
        public Order()
        {
            ID = 0;
            Placed = DateTime.Now;
            CustomerID = 0;
            ProductQuantities = new List<OrderRecord>();
        }

        /// <summary>
        /// Adds a given quantity of a product to this order.
        /// </summary>
        /// <param name="productId">The <see cref="Product"/> ID</param>
        /// <param name="productQuantity">The quantity to add. Must be greater than zero.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="productQuantity"/> parameter was zero or negative.</exception>
        public bool Add(int productId, decimal productQuantity)
        {
            if (ProductQuantities == null) return false;
            if (productQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(productQuantity));
            if (ProductQuantities.Count((r) => r.ProductID == productId) == 1)
            {
                var q = ProductQuantities.First((r) => r.ProductID == productId);
                q.Quantity += productQuantity;
            }
            else
                ProductQuantities.Add(new OrderRecord()
                {
                    ProductID = productId,
                    Quantity = productQuantity
                });
            return true;
        }

        /// <summary>
        /// Subtracts a given quantity of a product from this order.
        /// </summary>
        /// <param name="productId">The <see cref="Product"/> ID</param>
        /// <param name="productQuantity">The quantity to subtract. Must be greater than zero.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="productQuantity"/> parameter was zero or negative.</exception>
        public bool Subtract(int productId, decimal productQuantity)
        {
            if (ProductQuantities == null) return false;
            if (productQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(productQuantity));
            if (ProductQuantities.Count((r) => r.ProductID == productId) == 1)
                return false;
            var q = ProductQuantities.First((r) => r.ProductID == productId);
            q.Quantity -= productQuantity;
            return true;
        }

        /// <summary>
        /// Removes a product from this order.
        /// </summary>
        /// <param name="productId">The <see cref="Product"/> ID</param>
        /// <returns>True on success, false on failure.</returns>
        public bool Remove(int productId)
        {
            if (ProductQuantities == null) return false;
            return ProductQuantities.Remove(ProductQuantities.First((r) => r.ProductID == productId));
        }

        /// <summary>
        /// The total quantity of all products in this order.
        /// </summary>
        /// <exception cref="NullReferenceException"><see cref="ProductQuantities"/> was null.</exception>
        public decimal TotalQuantity => ProductQuantities != null ? ProductQuantities.Sum((p) => p.Quantity)
                                        : throw new NullReferenceException();

    }

    public class OrderDTO : EntityDTO<Order>
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public DateTime Placed { get; set; }
        public DateTime? Fullfilled { get; set; } = null;

        public IList<OrderRecord> ProductQuantities { get; set; } = null;

        public OrderDTO()
        { }

        public OrderDTO(Order order) : base(order)
        { }

        protected override Order ToEntity()
        {
            return new Order()
            {
                ID = ID,
                CustomerID = CustomerID,
                Placed = Placed,
                Fullfilled = Fullfilled,
                ProductQuantities = ProductQuantities
            };
        }

        protected override bool FromEntity(Order source)
        {
            ID = source.ID;
            CustomerID = source.CustomerID;
            Placed = source.Placed;
            Fullfilled = source.Fullfilled;
            ProductQuantities = source.ProductQuantities;
            return true;
        }
    }
}
