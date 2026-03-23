using ClinicalCompetence.Api.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger (dev)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory repository (swap with EF Core + SQL Server later)
builder.Services.AddSingleton<IRepository, InMemoryRepository>();

// If Angular is hosted separately, enable CORS (adjust origins!)
builder.Services.AddCors(o =>
{
    o.AddPolicy("spa", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true) // tighten in production
    );
});

var app = builder.Build();

// If behind IIS / reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();      // serves Angular build if copied into wwwroot
app.UseRouting();
app.UseCors("spa");
app.MapControllers();

// SPA fallback: any non-API route -> /index.html (Angular)
app.MapFallbackToFile("index.html");

app.Run();