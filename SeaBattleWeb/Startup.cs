using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSignalR();
        
        services.AddDbContext<ProfileContext>(
            opt => opt.UseInMemoryDatabase("Profiles"));
        services.AddDbContext<CompetitionContext>(
            opt => opt.UseInMemoryDatabase("Fields"));
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
        services.AddCors();

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

        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2),
            AllowedOrigins = { Configuration["AllowedHosts"] }
        });
        
        app.UseCors(opt => opt.AllowAnyOrigin());
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<RoomHub>("/play");
            endpoints.MapHub<TokenHub>("/create");
            endpoints.MapControllers();
        });
    }
}