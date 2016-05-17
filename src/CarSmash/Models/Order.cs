using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarSmash.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public double Total { get; set; }
        public string stripeChargeId { get; set; }
        public string stripeResponseJson { get; set; }
        // Status is a quick way to check if billed and completed properly.
        // 0 is not completed
        // 1 is billed
        // 2 is shipped?
        public int Status { get; set; }
        public string Message { get; set; }

        public void Charge()
        {
            
        }
    }
}
