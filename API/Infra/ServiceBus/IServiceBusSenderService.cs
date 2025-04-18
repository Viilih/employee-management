namespace API.Infra.ServiceBus;

public interface IServiceBusSenderService
{
    Task SendAsync(object message, CancellationToken cancellationToken);
    ValueTask DisposeAsync();
}