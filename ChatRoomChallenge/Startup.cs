using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomChallenge.Data;
using ChatRoomChallenge.Hubs.ChatCenter;
using ChatRoomChallenge.Hubs.ChatEvent;
using ChatRoomChallenge.Models;
using ChatTask.Hubs;
using Entities;
using Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQConsumer;
using Repository;
using Services;
using ServicesInterface;

namespace ChatRoomChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<StockSettings>(Configuration.GetSection("StockService"));

            services.AddDbContext<ApplicationDbContext>(config =>
            {
                // for in memory database  
                config.UseInMemoryDatabase("MemoryBaseDataBase");
            });

            services.AddDbContext<ChatDbContext>(config =>
            {
                // for in memory database  
                config.UseInMemoryDatabase("ChatMemoryBaseDataBase");
            });

            services.AddIdentity<AppUser, IdentityRole>(config =>
            {
                // User defined password policy settings.  
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Cookie settings   
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "DemoProjectCookie";
                config.LoginPath = "/Home/Login"; // User defined login path  
                config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

            services.AddSingleton<EventDispacher>();
            services.AddSingleton<IChatCenterController, ChatCenterController>();
            services.AddSingleton<IStockService, StockService>();
            services.AddTransient<PostRabbitMQHostedService>();
            services.AddTransient<IMessageService>(x => new MessageService( x.GetService<ChatDbContext>() ));
            services.AddControllersWithViews();
            services.AddHostedService<ConsumeRabbitMQHostedService>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });

            var service = app.ApplicationServices.GetService<IHubContext<ChatHub>>();
            var dispacherService = app.ApplicationServices.GetService<EventDispacher>();
            new ChatEventController(service, dispacherService);
        }
    }
}
