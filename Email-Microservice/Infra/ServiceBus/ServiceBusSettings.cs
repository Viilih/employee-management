namespace Email_Microservice.Infra.ServiceBus;

public class ServiceBusSettings
{
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
}