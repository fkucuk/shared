using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CoverletIssueRaiser
{

    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _emailService;

        public MessageService(ILogger<MessageService> emailService)
        {
            _emailService = emailService;
        }
        public Task SendAsync(string message)
        {
            _emailService.LogInformation($"mail sent from service: {message}");
            return Task.FromResult(0);

        }
    }

    public interface IMessageEventSender
    {

        Task Send(SendMessageEvent messageEvent);
    }

    public class RabbitConfig
    {

        public string RabbitMqConnectionString { get; set; }
        public string Host { get; set; }
        public string Uri { get; set; }
        public int Port { get; set; }
        public string Queue { get; set; }
        public bool IsUniqQueue { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class MessageConsumer : IConsumer<SendMessageEvent>
    {
        private readonly IMessageService _messageService;
        private readonly Logger<MessageConsumer> _logger;

        public MessageConsumer(IMessageService messageService, Logger<MessageConsumer> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SendMessageEvent> context)
        {
            _logger.LogInformation($"{context.Message.Message} has sent!");
            await _messageService.SendAsync(context.Message.Message);

        }
    }


    public interface IMessageService
    {
        Task SendAsync(string message);

    }
    public interface IBusMessage
    {
    }
    public class SendMessageEvent : IBusMessage
    {
        public string Message { get; set; }

    }

    public class MessageMassTransitService :
        IHostedService
    {
        readonly IBusControl _bus;
        private readonly ILogger<MessageMassTransitService> _logger;

        public MessageMassTransitService(IBusControl bus, ILogger<MessageMassTransitService> logger)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MessageMassTransitService is Started");
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MessageMassTransitService is Stoped");
            return _bus.StopAsync(cancellationToken);
        }


    }

    public class SendMessageRequest
    {
        public string Message { get; set; }
    }
}
