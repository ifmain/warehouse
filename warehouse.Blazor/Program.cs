using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using warehouse.Shared;
using warehouse.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app"); 

var apiUrl = builder.Configuration["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
var apiAddress = new Uri(apiUrl);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = apiAddress });

builder.Services.AddRefitClient<IWarehouseApi>()
    .ConfigureHttpClient(c => c.BaseAddress = apiAddress);

await builder.Build().RunAsync();