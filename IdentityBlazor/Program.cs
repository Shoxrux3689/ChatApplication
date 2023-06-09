using Blazored.LocalStorage;
using IdentityBlazor;
using IdentityBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri("https://localhost:7044/")
});
builder.Services.AddScoped<RequestService>();

await builder.Build().RunAsync();
