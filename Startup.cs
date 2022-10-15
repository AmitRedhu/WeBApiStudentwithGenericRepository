using EcommerceShoppingApp_DataAccess.Repository;
using EcommerceShoppingApp_DataAccess.Repository.IRepository;
using EcommerceShoppingApp_Utility;
using EcommerceShoppingApp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceShoppingApp
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<Iunitofwork, unitofwork>();

            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<StripeSettings>(Configuration.GetSection("stripe"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            //services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/login";
                options.LogoutPath = $"/Identity/Account/logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "1130631177837479";
                options.AppSecret = "46d6f9dc201a96b9e47d5ecaf26efc32";
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "284358709821-n4tcntak4qnvbacmlpg9b8q3qohtc67l.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
            app.UseSession();
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
