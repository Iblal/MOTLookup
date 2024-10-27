using MOTLookup.Infrastructure.IClients;
using MOTLookup.Models.Settings;
using MOTLookup.Presentation.Data;
using MOTLookup.Service.IServices;
using MOTLookup.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<IMOTLookupService, MOTLookupService>();
builder.Services.AddSingleton<IMOTApiClient, MOTApiClient>();

builder.Services.Configure<MotApiSettings>(builder.Configuration.GetSection("MotApiSettings"));

var motApiSettings = builder.Configuration.GetSection("MotApiSettings").Get<MotApiSettings>();

builder.Services.AddHttpClient<IMOTApiClient, MOTApiClient>(client =>
{
    client.BaseAddress = new Uri(motApiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("x-api-key", motApiSettings.ApiKey);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
