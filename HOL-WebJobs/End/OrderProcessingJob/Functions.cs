using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using SendGrid;

namespace OrderProcessingJob
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static void PutTestOrderInQueue(
            [Queue("orders")] out Order testOrder,
            TextWriter log)
        {
            testOrder = new Order()
            {
                Id = Guid.NewGuid().ToString("N"),
                BuyerEmail = "jason@jasonhaley.com",
                BuyerName = "Jason A Haley",
                OrderPlacedUtc = DateTime.UtcNow,
                Products = new List<Product>
                {
                    new Product()
                    {
                        Id = "Product1",
                        Name = "Product #1",
                        Price = 1.5,
                        Quantity = 5
                    },
                    new Product()
                    {
                        Id = "Product2",
                        Name = "Product #2",
                        Price = 2.1,
                        Quantity = 2
                    }
                }
            };

            // log message
            log.WriteLine("Queued test order");
        }

        public static void ProcessOrder(
            [QueueTrigger("%OrdersQueueName%")] Order order,
            [Blob("%OrdersContainerName%/{Id}")] out Order orderToSave,
            [Table("%ProductsTableName%")] ICollector<ProductsOrderedEntity> productsOrderedTable,
            [SendGrid] SendGridMessage message,
            TextWriter log
            )
        {
            // Save the order in blob storage
            orderToSave = order;

            // Save the ordered product quantities to table storage
            foreach (var product in order.Products)
            {
                productsOrderedTable.Add(
                    new ProductsOrderedEntity(order.Id, product, order.OrderPlacedUtc));
            }

            // Send and email to the buyer
            message.AddTo(order.BuyerEmail);
            message.Subject = $"Thanks for your order (#{order.Id})!";
            message.Text = $"{order.BuyerName}, we've received your order.";

            // log order processed
            log.WriteLine($"Processed {order.Products.Count} products");
        }

        public static void HandlePoisonMessage(
            [QueueTrigger("%OrdersQueueName%-poison")] string orderJson,
            [SendGrid] SendGridMessage message,
            TextWriter log)
        {
            // Send and email to the admin
            message.AddTo(ConfigurationManager.AppSettings["AdminEmail"]);
            message.Subject = "Order Failed Notification";
            message.Text = orderJson;

            // log poison message
            log.WriteLine($"Poison Order: {orderJson}");
        }

        [NoAutomaticTrigger]
        public static void PutPoisonOrderInQueue(
            [Queue("orders")] out Order testOrder,
            TextWriter log)
        {
            testOrder = new Order()
            {
                Id = Guid.NewGuid().ToString("N"),
                BuyerEmail = "<your email here>",
                BuyerName = "<your name here>",
                OrderPlacedUtc = DateTime.UtcNow
            };

            // log message
            log.WriteLine("Queued order");
        }

        public static void HandleErrors(
            [ErrorTrigger("00:05:00", 10)] TraceFilter filter,
            [SendGrid] ref SendGridMessage message,
            TextWriter log)
        {
            // Send and email to the admin
            message.AddTo(ConfigurationManager.AppSettings["AdminEmail"]);
            message.Subject = "WebJob Error Notification";
            message.Text = filter.GetDetailedMessage(5);

            // log last 5 detailed errors to the Dashboard
            log.WriteLine(filter.GetDetailedMessage(5));

        }

        [NoAutomaticTrigger]
        public static void PutTwoPoisonOrdersInQueue(
            [Queue("orders")] ICollector<Order> testOrders,
            TextWriter log)
        {
            for (int i = 0; i < 2; i++)
            {
                testOrders.Add(new Order()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    BuyerEmail = "<your email here>",
                    BuyerName = "<your name here>",
                    OrderPlacedUtc = DateTime.UtcNow
                });
            }

            // log message
            log.WriteLine("Queued order");
        }
    }
}
