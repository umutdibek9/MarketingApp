﻿namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate {  get; set; }
        public String? BuyerId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public OrderStatus OrderStatus { get; set; }
        public Address? Address { get; set; }
        public string? FailureMesage { get; set; }

    }
    public enum OrderStatus
    {
        Suspend,
        Success,
        Fail,
        Complete
    }
}
