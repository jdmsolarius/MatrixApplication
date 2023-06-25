using MatrixApp.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.IO;

namespace MatrixApp.CoreLogic
{
    public static class ServiceProviderFactory
    {
        public static IServiceCollection getServiceProvider()
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddMvcCore();
            serviceCollection.AddLogging();
            serviceCollection.AddMemoryCache(t=>t.CompactionPercentage = 20);
            serviceCollection.AddWebEncoders();
            serviceCollection.AddAntiforgery();
            serviceCollection.AddLocalization();
            serviceCollection.AddDbContext<AzazeldbContext>(ServiceLifetime.Transient);
            serviceCollection.BuildServiceProvider();
            return serviceCollection;
        }
    }
}
