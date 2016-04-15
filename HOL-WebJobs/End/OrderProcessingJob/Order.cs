using System;
using System.Collections.Generic;

namespace OrderProcessingJob
{
    public class Order
    {
        public string Id { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerCell { get; set; }
        public DateTime OrderPlacedUtc { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
