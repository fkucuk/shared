using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mail;
using CoverletIssueRaiser;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ApiTests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            bool isRabbitFake = false;
            builder.ConfigureTestServices(services =>
            {

                if (isRabbitFake)
                {
                    services.AddSingleton(provider => Bus.Factory.CreateUsingInMemory(configure =>
                    {
                        configure.ReceiveEndpoint("testendpoint", endpoint =>
                        {
                            endpoint.Consumer<MessageConsumer>(provider);
                            EndpointConvention.Map<SendMessageEvent>(endpoint.InputAddress);
                        });
                    }));
                    services.AddSingleton<IHostedService, MessageMassTransitService>();
                }
            });

        }
    }
}
