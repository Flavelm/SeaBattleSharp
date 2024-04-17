using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeaBattleWeb.Context;
using SeaBattleWeb.Models.Play;
using SeaBattleWeb.Services;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddDbContext<UsersContext>(
            opt => opt.UseInMemoryDatabase("Users"));
        services.AddDbContext<ProfileContext>(
            opt => opt.UseInMemoryDatabase("Profiles"));
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
        services.AddScoped<PlayFieldService>();
        services.AddTransient<FieldService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
            app.UseSwagger();
            app.UseSwaggerUI();
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
        
        //app.UseHttpsRedirection();
        //app.UseStaticFiles();

        app.UseCors(opt => opt.AllowAnyOrigin());
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}