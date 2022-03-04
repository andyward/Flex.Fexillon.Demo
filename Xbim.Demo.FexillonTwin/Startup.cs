using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.Json.Serialization;
using Xbim.Demo.FexillonTwin.Helpers;

namespace Xbim.Demo.FexillonTwin
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
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Xbim.Demo.FexillonTwin.xml");
            services.AddControllers()
                .AddNewtonsoftJson(opt => 
                {
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                }
                );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Xbim.Demo.FexillonTwin", Version = "v1" });
                c.IncludeXmlComments(filePath);
            });

            // Provides the xbim Flex services (IFlexClientsProvider etc)
            services.AddFlexLogInServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Xbim.Demo.FexillonTwin v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            
            // Sets up xbim flex master authentication using Flex config in appSettings 
            app.UseXbimFlex();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
