using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Threading.Tasks;

namespace CarSmash.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual Post Post { get; set; }
        public string Body { get; set; }
    }
}
