using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (use Azure Storage to save logs).
builder.Logging.ClearProviders(); // To override the default set of logging providers added
builder.Logging.AddAzureWebAppDiagnostics(); // add the required logging providers

// Add services to the container.
builder.Services.AddHttpClient();
// Add Jwt authentication (Azure).
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
        };
    });
// Require specified scope for authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("require_exchange_scope", policy =>
    {
        policy.RequireClaim("scope", "exchange");
    });
});
// Configure azure file logging details.
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "azure-diagnostics-";
    options.FileSizeLimit = 10 * 1024;
    options.RetainedFileCountLimit = 5;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Ryan Techno - Exchange Rate API",
        Description = "An live exchange rate querying API produced by Ryan Techno.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        },
    });

    // using System.Reflection to get documentation comments.
    // But this is not supported during deployment / publish to Azure App Service (https://stackoverflow.com/questions/59619433/referenced-assemblies-xml-docs-not-copied-during-publish)

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

// Use swagger in production.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
