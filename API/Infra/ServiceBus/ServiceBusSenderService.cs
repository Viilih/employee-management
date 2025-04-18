using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace API.Infra.ServiceBus;

public class ServiceBusSenderService : IAsyncDisposable, IServiceBusSenderService
{
    private readonly string _queueName;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBusSenderService(IOptions<ServiceBusSettings> settings)
    {
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        _queueName = settings.Value.QueueName;
        _client = new ServiceBusClient(settings.Value.ConnectionString, clientOptions);
        _sender = _client.CreateSender(_queueName);
    }

    public async Task SendAsync(object message, CancellationToken cancellationToken)
    {
        try
        {
            var messageSerialized = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageSerialized)
            {
                ContentType = "application/json"
            };
            Console.WriteLine($"Sending message to queue: {_queueName}"); // Add logging
            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            Console.WriteLine("Message sent successfully."); 
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending message: {e.Message}");
            throw;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}