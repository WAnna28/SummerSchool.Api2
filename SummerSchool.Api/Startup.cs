using EF.Dal.EfStructures;
using EF.Dal.Initialization;
using EF.Dal.Repos;
using EF.Dal.Repos.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SummerSchool.Services;
using SummerSchool.Services.Logging;

namespace SummerSchool.Api
{
    public class Startup
    {
        // The constructor takes an instance of the IConfiguration interface that was created by the Host.
        // CreateDefaultBuilder method in the Program.cs file and assigns it to the Configuration property
        // for use elsewhere in the class. The constructor can also take an instance of the IWebHostEnvironment
        // and/or the ILoggerFactory, although these are not added in the default template.
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SummerSchool.Api", Version = "v1" });
            });

            var connectionString = Configuration.GetConnectionString("SummerSchoolDB");
            services.AddDbContextPool<ApplicationDbContext>(
                 options => options.UseSqlServer(connectionString,
                     sqlOptions => sqlOptions.EnableRetryOnFailure().CommandTimeout(60)));

            services.AddScoped(typeof(IAppLogging<>), typeof(AppLogging<>));

            services.AddScoped<ICarRepo, CarRepo>();
            services.AddScoped<ICreditRiskRepo, CreditRiskRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();
            services.AddScoped<IMakeRepo, MakeRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                if (Configuration.GetValue<bool>("RebuildDataBase"))
                {
                    SampleDataInitializer.InitializeData(context);
                }
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SummerSchool.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
