using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using CampusEats.Persistence;

namespace CampusEats;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        // JSON & Database
        builder.Services.ConfigureHttpJsonOptions(opt => 
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<CampusEatsContext>(options =>
            options.UseNpgsql(dataSource));

        // Security & Swagger
        builder.AddJwtAuthentication();
        builder.AddSwaggerConfig();
        
        builder.Services.AddCors(options => options.AddPolicy("AllowAll", 
            p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
        builder.Services.AddAuthorization();
    }

    private static void AddSwaggerConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
            });
        });
    }
}