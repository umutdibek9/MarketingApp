using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
    [Owned]
    public class Address
    {
        public string Line { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
    }
}
