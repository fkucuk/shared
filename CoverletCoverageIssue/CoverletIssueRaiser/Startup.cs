using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CoverletIssueRaiser
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            var rabbitConfig = new RabbitConfig();

            Configuration.Bind("RabbitMq", rabbitConfig);

            services.AddScoped<MessageConsumer>();
            services.AddTransient<IMessageService, MessageService>();


            services.AddMassTransit(x =>
            {

                x.AddConsumer<MessageConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {


                var host = cfg.Host(new Uri(rabbitConfig.Uri), hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitConfig.UserName);
                    hostConfigurator.Password(rabbitConfig.Password);
                });

                cfg.UseExtensionsLogging(provider.GetService<ILoggerFactory>());

                cfg.ReceiveEndpoint(host, rabbitConfig.Queue, e =>
                {
                    e.PrefetchCount = 16;

                    e.UseMessageRetry(x => x.Exponential(10, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(100), TimeSpan.FromMinutes(1)));

                    e.Consumer<MessageConsumer>(provider);

                    EndpointConvention.Map<SendMessageEvent>(e.InputAddress);
                });
            }));
            services.AddSingleton<IHostedService, MessageMassTransitService>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseMvc();
        }
    }

}
