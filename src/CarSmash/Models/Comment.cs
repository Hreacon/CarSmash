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
        public int CommentId { get; set; }
        public int userId { get; set; }
        public virtual Post post { get; set; }
        public string body { get; set; }

        public Comment()
        {
        }
    }
}
