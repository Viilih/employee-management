using Azure.Messaging.ServiceBus;

namespace Email_Microservice.Infra.ServiceBus;

public interface IServiceBusConsumerService
{
    Task StartProcessingAsync();
    // Task StopProcessingAsync();
}