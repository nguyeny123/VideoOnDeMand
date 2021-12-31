using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using VideoOnDemand.Data.Data;
using VideoOnDemand.Data.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VideoOnDemand.UI.Repositories;
using VideoOnDemand.UI.Models.DTOModels;
using VideoOnDemand.Data.Services;
using VideoOnDemand.Admin.Services;
using VideoOnDemand.Data.Migrations;

namespace VideoOnDemand.UI
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
            // services.AddDbContext<VODContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            var IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            var connectionString = IsDevelopment ? Configuration.GetConnectionString("DefaultConnection") : GetHerokuConnectionString();

            services.AddDbContext<VODContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<VODContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddSingleton<IReadRepository, MockReadRepository>();
            services.AddScoped<IReadRepository, SqlReadRepository>();
            services.AddTransient<IDbReadService, DbReadService>();
            services.AddTransient<IDbWriteService, DbWriteService>();
            services.AddTransient<IUserService, UserService>();
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Video, VideoDTO>();
                cfg.CreateMap<Instructor, InstructorDTO>()
                .ForMember(dest => dest.InstructorName,
                src => src.MapFrom(s => s.Name))
                .ForMember(dest => dest.InstructorDescription,
                src => src.MapFrom(s => s.Description))
                .ForMember(dest => dest.InstructorAvatar,
                src => src.MapFrom(s => s.Thumbnail));
                cfg.CreateMap<Download, DownloadDTO>()
                .ForMember(dest => dest.DownloadUrl,
                src => src.MapFrom(s => s.Url))
                .ForMember(dest => dest.DownloadTitle,
                src => src.MapFrom(s => s.Title));
                cfg.CreateMap<Course, CourseDTO>()
                .ForMember(dest => dest.CourseId, src =>
                src.MapFrom(s => s.Id))
                .ForMember(dest => dest.CourseTitle,
                src => src.MapFrom(s => s.Title))
                .ForMember(dest => dest.CourseDescription,
                src => src.MapFrom(s => s.Description))
                .ForMember(dest => dest.MarqueeImageUrl,
                src => src.MapFrom(s => s.MarqueeImageUrl))
                .ForMember(dest => dest.CourseImageUrl,
                src => src.MapFrom(s => s.ImageUrl));
                cfg.CreateMap<Module, ModuleDTO>()
                .ForMember(dest => dest.ModuleTitle,
                src => src.MapFrom(s => s.Title));
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static string GetHerokuConnectionString()
        {
            string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            var databaseUri = new Uri(connectionUrl);

            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};sslmode=Require;Trust Server Certificate=True;";
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VODContext context)
        {
            context.Database.Migrate();
            DbInitializer.Initialize(context);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
                endpoints.MapRazorPages();
            });
        }
    }
}
