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
    [Table("Posts")]
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
