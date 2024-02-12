using ChatGPTAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the OpenAIService and configure it
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.Configure<OpenAIServiceOptions>(builder.Configuration.GetSection("OpenAI"));

// Register the InAppFileSaver service as scoped
builder.Services.AddScoped<InAppFileSaverService>(); // This matches the class name
 // Add this line to register InAppFileSaver

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
