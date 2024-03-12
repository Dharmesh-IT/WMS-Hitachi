using Application.Services;
using Application.Services.User;
using AutoMapper;
using DatabaseLibrary.SQL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using WMS.Data;
using WMS.Web.Framework.Infrastructure.Extentsion;
using WMSWebApp.HitachiProvider;
using WMSWebApp.Models;
using WMSWebApp.Wrapper;
namespace WMSWebApp
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("default");



            //services.AddDbContext<AppDBContext>(c =>
            //{
            //    c.UseSqlite("Data Source=blog.db");
            //});
            //services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<WMSObjectContext>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";

            //});
            //Initialize the mapper
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ViewToDomainModelMappingProfile());

            });
            services.AddDbContext<WMSObjectContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("default")).UseLazyLoadingProxies();

            });
           
            services.AddResponseCompression();
            services.AddHttpClient();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IOTimeout = TimeSpan.FromMinutes(20);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>
                (
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.User.RequireUniqueEmail = true;
                    }


                )
                .AddRoleManager<RoleManager<IdentityRole>>()
               .AddEntityFrameworkStores<WMSObjectContext>()
               .AddDefaultTokenProviders();
            //services.AddTransient<IDbContext, WMSObjectContext>();
            //services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddSingleton<IMapper>(new Mapper(config));
            services.AddSingleton<IAdoConnection>(new AdoConnection(connectionString));
            services.AddSingleton<IMemoryCacheWrapper, MemoryCacheWrapper>();
   		    services.AddSingleton<IFileProvider>(new PhysicalFileProvider(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot")));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHitachiConnection, HitachiConnention>();
            //services.AddTransient<ICompanyHelper, CompanyHelper>();
            //services.AddTransient<IBranchHelper, BranchHelper>();
            //services.AddTransient<ICustomerHelper,CustomerHelper>();
            //services.AddTransient<IItemHelper, ItemHelper>();
            //services.AddTransient<ISubItemHelper, SubItemHelper>();
            services.AddScoped<IIntrasitHelper, IntrasitHelper>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;//6
                options.Password.RequiredUniqueChars = 1;//1

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllersWithViews().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                opt.JsonSerializerOptions.AllowTrailingCommas = true;
                opt.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString;
                opt.JsonSerializerOptions.IncludeFields = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5);

                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/Login");
                options.SlidingExpiration = true;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddSingleton<IFileProvider>( new PhysicalFileProvider(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot")));
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;

            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
			services.AddResponseCompression();
            services.AddHttpClient();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WMS API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                    } });
            });
            //services.AddControllers();
            //services.AddExceptionless("Bzq30fE6Q3wiclEacIWD2WDRepels6b2JDPkgG5f");
            return services.ConfigureApplicationServices(Configuration, _webHostEnvironment);
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
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                });
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMSAPI v1"));
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            //app.UseExceptionless();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
