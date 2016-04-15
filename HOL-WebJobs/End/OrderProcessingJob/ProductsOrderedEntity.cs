using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace OrderProcessingJob
{
    public class ProductsOrderedEntity : TableEntity
    {
        public ProductsOrderedEntity()
        { }

        public ProductsOrderedEntity(string orderId,
            Product product,
            DateTime orderedUtc)
        {
            SetRowKey(orderId, product, orderedUtc);

            OrderId = orderId;
            Id = product.Id;
            Name = product.Name;
            Quantity = product.Quantity;
            Price = product.Price;
        }

        public string OrderId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public void SetRowKey(string orderId,
            Product product,
            DateTime orderedUtc)
        {
            // Use a reverse data schema to keep rows sorted chronologically 
            long ticks = DateTimeOffset.MaxValue.Ticks - orderedUtc.Ticks;

            RowKey = $"{ticks}_{orderId}_{product.Id}";
            PartitionKey = orderId;
        }
    }
}
