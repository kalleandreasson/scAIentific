using ChatGPTAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.DescribeAllParametersInCamelCase();
    options.CustomSchemaIds(type=>type.ToString());
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "scAIentific",
        Description = "scAIentific API"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                }
            },
            new List < string > ()
        }
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretPriv"] ?? string.Empty))
    };
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register the OpenAIService and configure it
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.Configure<OpenAIServiceOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient<AssistantService>();
builder.Services.AddHttpClient<ChatService>();
builder.Services.Configure<DatabaseServiceOptions>(builder.Configuration.GetSection("DatabaseServiceOptions"));
builder.Services.AddSingleton<MongoDBService>();

// Register the InAppFileSaver service as scoped
builder.Services.AddScoped<EmailSenderService>();
builder.Services.AddScoped<InAppFileSaverService>(); // This matches the class name
builder.Services.AddScoped<ChatService>(); // This matches the class name
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
