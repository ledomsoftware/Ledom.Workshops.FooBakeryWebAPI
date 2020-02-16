using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ledom.Workshops.FooBakeryWebAPI.Entities
{
    /// <summary>
    /// Represents a product in the catalog.
    /// </summary>
    public class Product : Entity
    {
        /// <summary>
        /// ID (primary key) of this product.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Display name of this product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short description of this product.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Price of this product in EUR.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="id">ID of this product. Must be positive.</param>
        /// <param name="name">Name of this product, must not be null.</param>
        /// <param name="price">Price of this product, must be positive.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Either <paramref name="id"/> or <paramref name="price"/> was negative or 0.</exception>
        public Product(int id, string name, decimal price)
        {
            if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));
            ID = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
            Price = price;
        }

        /// <summary>
        /// Used while deserializing.
        /// </summary>
        public Product()
        {
            ID = 0;
            Name = string.Empty;
            Price = 0;
        }
    }

    public class ProductDTO : EntityDTO<Product>
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public ProductDTO()
        { }

        public ProductDTO(Product product) : base(product)
        { }

        protected override Product ToEntity()
        {
            return new Product()
            {
                ID = ID,
                Name = Name,
                Description = Description,
                Price = Price
            };
        }

        protected override bool FromEntity(Product source)
        {
            ID = source.ID;
            Name = source.Name;
            Description = source.Description;
            Price = source.Price;
            return true;
        }
    }
}
