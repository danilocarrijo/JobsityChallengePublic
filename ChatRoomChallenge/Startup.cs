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
using LinqToDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

            
            services.Configure<BotSettings>(Configuration.GetSection("BotSettings"));

            var dbtype = Configuration.GetSection("dbType");

            switch (dbtype.Value)
            {
                case "MEM":
                    services.AddDbContext<ApplicationDbContext>(config =>
                    {
                        config.UseInMemoryDatabase("IdentiydataBase");
                    });

                    services.AddDbContext<ChatDbContext>(config =>
                    {
                        config.UseInMemoryDatabase("MemoryBaseDataBase");
                    });
                    break;
                case "DB":
                    services.AddDbContext<ApplicationDbContext>(config =>
                    {
                        config.UseMySql(Configuration.GetConnectionString("dbConn"), builder =>
                        {
                            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        });
                    });

                    services.AddDbContext<ChatDbContext>(config =>
                    {
                        config.UseMySql(Configuration.GetConnectionString("dbConn"), builder =>
                        {
                            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        });

                    });
                    break;
            }


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
            services.AddTransient<IMessageService>(x => new MessageService( x.GetService<ChatDbContext>() ));
            services.AddControllersWithViews();
            services.AddHostedService<ConsumeRabbitMQHostedService>();
            services.AddSignalR();
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ChatDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        private static void UpdateIdentityDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                try
                {

                    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables(); 
                    var context2 = serviceScope.ServiceProvider.GetRequiredService<ChatDbContext>();
                    databaseCreator = (RelationalDatabaseCreator)context2.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
                catch (Exception){}
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UpdateIdentityDatabase(app);

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
