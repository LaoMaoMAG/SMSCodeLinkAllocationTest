using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace AdminPanel;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    
        // 配置API的HttpClient
        builder.Services.AddScoped(sp => new HttpClient 
        { 
            BaseAddress = new Uri(Config.WebAPIServer) // 后端API地址
        });
        
        await builder.Build().RunAsync();
    }
}