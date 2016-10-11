using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStore.Api.Models;
using MusicStore.Api.Models.Dto;
using AutoMapper;

namespace MusicStore.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add CORS support
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyClient",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });
            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = null;
            });

            // Add EF services to the service container
            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                .AddDbContext<MusicStoreContext>(options => {
                    options.UseSqlite("Data Source=music-db.sqlite");
                });

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<MusicStoreContext>()
                    .AddDefaultTokenProviders();

            // Configure Auth
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("app-ManageStore", new AuthorizationPolicyBuilder().RequireClaim("app-ManageStore", "Allowed").Build());
            });

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AlbumChangeDto, Album>();
                cfg.CreateMap<Album, AlbumChangeDto>();
                cfg.CreateMap<Album, AlbumResultDto>();
                cfg.CreateMap<AlbumResultDto, Album>();
                cfg.CreateMap<Artist, ArtistResultDto>();
                cfg.CreateMap<ArtistResultDto, Artist>();
                cfg.CreateMap<Genre, GenreResultDto>();
                cfg.CreateMap<GenreResultDto, Genre>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if(env.IsDevelopment() == true)
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDeveloperExceptionPage();
            // Initialize the sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapWhen(context =>
            {
                var path = context.Request.Path.Value;
                if (path.Contains("/api/")) return false;
                return (!path.Contains("."));
            }, aBranch =>
            {
                aBranch.Use((context, next) =>
                {
                    context.Request.Path = new PathString("/index.html");
                    return next();
                });
                aBranch.UseStaticFiles();
            });
            app.UseCors("AllowAnyClient");
            app.UseMvc();
        }
    }
}
