using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SendGrid;

namespace OrderProcessingJob
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JobHostConfiguration();

            config.NameResolver = new NameResolver();
            config.Tracing.Tracers.Add(new ColorConsoleTraceWriter(TraceLevel.Info));
            config.UseSendGrid(new SendGridConfiguration()
            {
                FromAddress = new MailAddress(
                        ConfigurationManager.AppSettings["AdminEmail"])
            });
            config.UseCore();

            config.UseDevelopmentSettings();

            var host = new JobHost(config);
            //host.Call(typeof(Functions).GetMethod("PutTestOrderInQueue"));
            //host.Call(typeof(Functions).GetMethod("PutPoisonOrderInQueue"));
            //host.Call(typeof(Functions).GetMethod("PutTwoPoisonOrdersInQueue"));
            host.RunAndBlock();
        }
    }
}
