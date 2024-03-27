using Frontend.Services;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Polly;
using System;

var builder = WebApplication.CreateBuilder(args);

// Set the EPPlus license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial if you have a commercial license.

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Retrieve the API Base URL from configuration
var apiBaseUrl = builder.Configuration.GetValue<string>("APIBaseUrl");

// Configure HttpClient for UploadFileService with the API Base URL from configuration
builder.Services.AddHttpClient<AssistantCreationService>(client => 
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
{
    TimeSpan.FromSeconds(1),
    TimeSpan.FromSeconds(5),
    TimeSpan.FromSeconds(10)
}));

// Configure HttpClient for ChatService with the API Base URL from configuration
builder.Services.AddHttpClient<ChatService>(client =>
{
    // Here we use the base address from the configuration, making it dynamic
    client.BaseAddress = new Uri(apiBaseUrl); 
});

builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<AssistantCreationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
