using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PaymentCompletedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }

        public string Address { get; set; }

        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> orderItems { get; set; }

    }
}
