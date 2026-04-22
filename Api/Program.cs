using System.Text;
using System.Linq;
using AutoMapper;
using FinancieraSoluciones.Api.BackgroundJobs;
using FinancieraSoluciones.Api.Middlewares;
using FinancieraSoluciones.Application.DependencyInjection;
using FinancieraSoluciones.Application.Mapping;
using FluentValidation;
using FluentValidation.AspNetCore;
using FinancieraSoluciones.Infraestructura.DependencyInjection;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using FinancieraSoluciones.Application.DTOs.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? string.Empty))
        };
    });

builder.Services
    .AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message));
        var message = string.Join(" | ", errors);
        return new OkObjectResult(ApiResponse<object>.Fail(message, 400));
    };
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<FinancieraSoluciones.Application.Validaciones.Finanzas.AbonarFichaRequestDtoValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinancieraSoluciones API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<FinancieraSolucionesProfile>(), typeof(FinancieraSolucionesProfile).Assembly);

builder.Services
    .AddFinancieraSolucionesApplication()
    .AddFinancieraSolucionesInfrastructure(builder.Configuration);

builder.Services.AddHostedService<ActualizarMoraBackgroundService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinancieraSoluciones API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseMiddleware<DomainExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapControllers();

app.Run();
