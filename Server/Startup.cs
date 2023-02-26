﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Server.Authentication;
using Server.Controller;
using Server.Model.Db;
using Server.Services;

namespace Server
{
    public class Startup
    {
        public const string CustomTokenScheme = nameof(CustomTokenScheme);

        public Startup(IConfiguration configuration)
        {
            
        }

        public IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GameDbContext>();
            services.AddAuthentication(o => 
                    o.DefaultScheme = GameAuthenticationDefaults.AuthenticationScheme)
                .AddGameAuthentication<UserService>(options =>
                {
                    options.Realm = "TicTacToe Game";
                    options.ForwardAuthenticate = GameAuthenticationDefaults.AuthenticationScheme;
                });
            services.AddAuthorization(options =>
            {

            });
            services.AddMvc();

            services.AddControllersWithViews();
            //services.AddAuthentication(JwtBearerDefailts.AuthenticationScheme);
            services.AddCors();
            services.AddSignalR();
            services.AddSingleton<IGameService, GameService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMatchStatisticsService, MatchStatisticsService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseDefaultFiles();
            //app.UseStaticFiles();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "DELETE")
                    .AllowCredentials();
            });

            app.UseRouting();

            app.UseTokenParserMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<GameHub>("/game");
            });
        }
    }

    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Claims
                .FirstOrDefault(x => x.Type == "username")?.Value;
        }
    }
}
