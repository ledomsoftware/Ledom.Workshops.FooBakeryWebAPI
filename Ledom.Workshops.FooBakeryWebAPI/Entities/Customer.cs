using System;
using System.ComponentModel.DataAnnotations;

namespace Ledom.Workshops.FooBakeryWebAPI.Entities
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer : Entity
    {
        /// <summary>
        /// The ID (primary key) of this customer.
        /// </summary>
        [Required]
        public int ID { get; set; }

        /// <summary>
        /// This customer's name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="id">Customer ID.</param>
        /// <param name="name">Customer name. Must not be null.</param>
        public Customer(int id, string name)
        {
            ID = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Used while deserializing
        /// </summary>
        public Customer()
        {
            ID = 0;
            Name = string.Empty;
        }
    }
    public class CustomerDTO : EntityDTO<Customer>
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public CustomerDTO()
        { }

        public CustomerDTO(Customer customer) : base(customer)
        { }

        protected override Customer ToEntity()
        {
            return new Customer()
            {
                ID = ID,
                Name = Name
            };
        }

        protected override bool FromEntity(Customer source)
        {
            ID = source.ID;
            Name = source.Name;
            return true;
        }
    }
}
