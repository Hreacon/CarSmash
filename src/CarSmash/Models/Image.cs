using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.AspNet.Mvc;

namespace CarSmash.Models
{
    [Table("Images")]
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        public string Url { get; set; }
        public virtual Product product { get; set; }

        public Image()
        {
        }
    }
}
