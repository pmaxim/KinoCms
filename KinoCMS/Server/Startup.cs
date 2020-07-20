using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using AutoMapper;
using Domain.Repositories;
using Domain.Repositories.Concrete;
using Hangfire;
using Hangfire.MemoryStorage;
using KinoCMS.Server.Data;
using KinoCMS.Server.HangfireJobs;
using KinoCMS.Server.Infrastructure;
using KinoCMS.Server.Models;
using Microsoft.VisualBasic;

namespace KinoCMS.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var cs = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EfDbContext>(options =>
                options.UseSqlServer(cs));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(cs));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddSigningCredential(Certificate.Get())
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddHangfire(x => x.UseMemoryStorage());
            services.AddHangfireServer();

            ConfigureDi(services);

            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        private static void ConfigureDi(IServiceCollection services)
        {
            services.AddScoped<IFilmRepository, EfFilmRepository>();

            services.AddScoped<IHangfireKinoKopilkaParser, HangfireKinoKopilkaParser>();

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AddHangfire(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private static void AddHangfire(IApplicationBuilder app)
        {
            //app.UseHangfireDashboard();

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { "critical", "default" },
                ServerName = "Hangfire:1"
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
#if DEBUG
            //RecurringJob.AddOrUpdate<IHangfireKinoKopilkaParser>(z => z.Run(), Cron.Minutely);
            //var jobId = BackgroundJob.Enqueue<IHangfireKinoKopilkaParser>(z => z.Run());
#endif
            var jobId = BackgroundJob.Enqueue<IHangfireKinoKopilkaParser>(z => z.Run());
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadSendSms>(z => z.Run(), "*/2 * * * *");
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadDisposition>(z => z.Run(), "*/5 * * * *");
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadAlarm>(z => z.Run(), Cron.Minutely);
            //RecurringJob.AddOrUpdate<IHangfireRemoveMmsImages>(z => z.Run(), Cron.Daily);
        }
    }
}
