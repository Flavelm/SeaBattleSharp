using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeaBattleWeb.Contexts;
using SeaBattleWeb.Hubs;
using SeaBattleWeb.Services;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb;

public class Startup(IConfiguration configuration)
{
    private const string CorsPolicy = "CorsPolicy";
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddCors(opt =>
        {
            opt.AddPolicy(CorsPolicy, builder =>
            {
                string? origins = Environment.GetEnvironmentVariable("ORIGINS");
                if (origins == null)
                    builder.AllowAnyOrigin();
                else
                    builder.WithOrigins(origins.Split(";"));
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
        });
        
        string? sqlServer = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            if (sqlServer != null)
                opt.UseMySql(sqlServer, ServerVersion.AutoDetect(sqlServer));
            else
                opt.UseInMemoryDatabase("Saves");
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
        
        services.AddAuthorization();
        services.AddAuthorizationBuilder()
            .AddPolicy("user_play", policy =>
                policy.RequireRole("user")
                    .RequireClaim("scope", "play_api"));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddWebSockets(opt =>
        {
            opt.KeepAliveInterval = TimeSpan.FromMinutes(2);
            opt.AllowedOrigins.Add(Configuration["AllowedHosts"]);
        });
        
        services.AddSignalR();
        
        services.AddSingleton<IRoomsService, RoomsService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddTransient<FieldServiceFactory>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCors(CorsPolicy);
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2),
            AllowedOrigins = { Configuration["AllowedHosts"] }
        });
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<RoomHub>("/api/play");
            endpoints.MapHub<TokenHub>("/api/create");
            endpoints.MapControllers();
        });
    }
}