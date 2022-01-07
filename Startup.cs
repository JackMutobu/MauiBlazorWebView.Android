using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWebView
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<HttpClient>();
            services.RegisterBlazorWebView();
        }

        public static IServiceProvider Initialize(CancellationToken appLifetime)
        {
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());

            var startup = Activator.CreateInstance(typeof(Startup)) as Startup;
            startup!.ConfigureServices(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }
    }
}
