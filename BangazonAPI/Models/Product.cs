using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        
        public int CustomerId { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        //trying to get the strings from the customer id and product type id, but doesn't look like we need it
        //public string ProductType { get; set; }
    }
}
