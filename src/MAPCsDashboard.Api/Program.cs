using System.Reflection;
using MAPCsDashboard.Api.Authentication;
using MAPCsDashboard.Api.Data;
using MAPCsDashboard.Api.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<ApiKeyOptions>()
    .Bind(builder.Configuration.GetSection(ApiKeyOptions.SectionName))
    .Validate(
        o => !string.IsNullOrWhiteSpace(o.Key)
            && !o.Key.StartsWith("REEMPLAZAR", StringComparison.OrdinalIgnoreCase),
        "Debe configurar ApiKey:Key con una clave real.")
    .ValidateOnStart();

builder.Services.AddAuthentication(ApiKeyAuthenticationDefaults.Scheme)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationDefaults.Scheme,
        _ => { });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IChequeRepository, SqlChequeRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catedral Software - API ERP",
        Version = "v1",
        Description = "API de consulta (solo lectura) para información del ERP."
    });
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Clave provista por Catedral Software. Encabezado: X-API-Key",
        Name = ApiKeyAuthenticationDefaults.HeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
        }] = Array.Empty<string>()
    });
    var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlName));
});

var app = builder.Build();

app.UseMiddleware<GetOnlyMiddleware>();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API ERP v1");
    options.RoutePrefix = "documentacion";
    options.DocumentTitle = "Documentación API ERP";
    options.DisplayRequestDuration();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/documentacion"))
    .AllowAnonymous()
    .ExcludeFromDescription();

app.Run();

public partial class Program;
