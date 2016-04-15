using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace OrderProcessingJob
{
    public class OrderBlobBinder : ICloudBlobStreamBinder<Order>
    {
        public Task<Order> ReadFromStreamAsync(Stream input,
            CancellationToken cancellationToken)
        {
            using (StreamReader reader = new StreamReader(input))
            {
                var jsonString = reader.ReadToEnd();
                var order = JsonConvert.DeserializeObject<Order>(jsonString);
                return Task.FromResult(order);
            }
        }

        public Task WriteToStreamAsync(Order value, Stream output,
            CancellationToken cancellationToken)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(output))
            using (var jsonTextWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonTextWriter, value);
            }
            return Task.FromResult<object>(null);
        }
    }
}
