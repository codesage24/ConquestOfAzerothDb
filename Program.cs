using ConquestOfAzerothDb.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ConquestOfAzerothDb;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        AddServices(builder.Services, builder.HostEnvironment);

        await builder.Build().RunAsync();
    }

    public static void AddServices(IServiceCollection services, IWebAssemblyHostEnvironment environment)
    {
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(environment.BaseAddress) });
    }
}
