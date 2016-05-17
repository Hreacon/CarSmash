using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSmash.ViewModels.Products
{
    public class CreateProduct
    {
        [Required]
        public string Name { get; set; }

        [Required]
         public string Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public IFormFile File { get; set; }

        
    }
}
