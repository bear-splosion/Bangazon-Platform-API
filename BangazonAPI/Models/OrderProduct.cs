﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        [Required]
        public int OrderIdId { get; set; }

        [Required]
        public int ProductId { get; set; }


    }
}
