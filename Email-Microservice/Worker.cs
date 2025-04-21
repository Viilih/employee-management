using Email_Microservice.Infra.ServiceBus;

namespace Email_Microservice;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceBusConsumerService _serviceBusConsumerService;

    public Worker(ILogger<Worker> logger, IServiceBusConsumerService serviceBusConsumerService)
    {
        _logger = logger;
        _serviceBusConsumerService = serviceBusConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
          
            await _serviceBusConsumerService.StartProcessingAsync(); 

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker alive at: {time}", DateTimeOffset.Now);
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker stopping gracefully.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Unhandled exception in Worker ExecuteAsync.");
        }
        finally
        {
            _logger.LogInformation("Worker ExecuteAsync finally block executing. Ensuring processor is stopped...");
            _logger.LogInformation("Worker finished.");
        }
    }
}