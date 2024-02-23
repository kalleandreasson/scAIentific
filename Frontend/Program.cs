using Frontend.Services;
using OfficeOpenXml;
using Polly;

 // Add this to Program.cs


var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;// or LicenseContext.Commercial if you have a commercial license.
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient<UploadFileService>(client => 
{
    client.BaseAddress = new Uri("http://localhost:5000/");
})
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
{
    TimeSpan.FromSeconds(1),
    TimeSpan.FromSeconds(5),
    TimeSpan.FromSeconds(10)
}));





builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<UploadFileService>();



// builder.Services.AddHttpClient<OpenAIService>();
// builder.Services.Configure<OpenAIServiceOptions>(builder.Configuration.GetSection("OpenAI"));

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
