using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DebtsRegister.Initialization.Simplified
{
    public class SimplifiedWebApiStartup
    {
        public IConfigurationRoot Configuration { get; }

        public SimplifiedWebApiStartup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            SimplifiedCoreInitializer.AddConfiguration(builder);

            this.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services) {
            SimplifiedCoreInitializer
                .AddDIServices(services, this.Configuration);
        }
        
        public void Configure(
            IApplicationBuilder app, IHostingEnvironment env
        ) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.Run(async (context) => {
                await context.Response.WriteAsync(""); });
        }
    }
}