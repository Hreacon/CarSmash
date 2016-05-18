using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CarSmash.Models
{
    [Table("Images")]
    [Serializable]
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        public string Url { get; set; }

        public virtual Product Product { get; set; }

        public Image()
        {

        }
        
    }
}