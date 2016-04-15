using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;

namespace OrderProcessingJob
{
    public class NameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            var settingValue = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = Environment.GetEnvironmentVariable(name);
            }
            return settingValue;
        }
    }
}
