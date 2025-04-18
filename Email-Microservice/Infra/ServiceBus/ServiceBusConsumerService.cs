using System.Text.Json;
using API.features.Employees.CreateEmployee;
using Azure.Messaging.ServiceBus;
using Email_Microservice.Infra.EmailCommService;
using Microsoft.Extensions.Options;

namespace Email_Microservice.Infra.ServiceBus;

public class ServiceBusConsumerService : IAsyncDisposable, IServiceBusConsumerService
{
    private readonly string _queueName;
    private readonly ILogger<Worker> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    private readonly IEmailService _emailService;

    public ServiceBusConsumerService(IOptions<ServiceBusSettings> settings, ILogger<Worker> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        _queueName = settings.Value.QueueName;
        _client = new ServiceBusClient(settings.Value.ConnectionString, clientOptions);
        _processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    public async Task StartProcessingAsync()
    {
        await _processor.StartProcessingAsync();
    }
    
     private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        _logger.LogInformation("Received message: SequenceNumber:{SequenceNumber} Body:{Body}", args.Message.SequenceNumber, body);

        CreateEmployeeMessage employeeMessage = null;
        try
        {
            employeeMessage = JsonSerializer.Deserialize<CreateEmployeeMessage>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (employeeMessage == null)
            {
                _logger.LogError("Failed to deserialize message body for SequenceNumber:{SequenceNumber}", args.Message.SequenceNumber);
                await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Could not deserialize message body.");
                return;
            }
            
            string subject = $"Welcome, {employeeMessage.EmployeeFirstName}!";
            string htmlBody = $@"
            <html>
            <body>
                <h1>Welcome aboard, {employeeMessage.EmployeeFirstName} {employeeMessage.EmployeeLastName}!</h1>
                <br/>
                <p>We are excited to have you join the {employeeMessage.DepartmentSector} department.</p>
                <p>Your employee ID is: {employeeMessage.EmployeeId}</p>
                <p>Your registered email is: {employeeMessage.EmployeeEmail}</p>
                {(employeeMessage.CoordinatorId.HasValue ? $"<p>Your coordinator is: {employeeMessage.CoordinatorFirstName} {employeeMessage.CoordinatorLastName} ({employeeMessage.CoordinatorEmail})</p>" : "<p>Your coordinator information will be provided soon.</p>")}
                <br/>
                <p>This email was automatically generated.</p>
            </body>
            </html>";

            // If you want to test receiving your email you should either put your email or 
            // Assign your email to an employee 
            // string recipientEmail = employeeMessage.EmployeeEmail
            string recipientEmail = "randomemail@gmail.com";
            await _emailService.SendEmail(recipientEmail, subject, htmlBody);
            _logger.LogInformation("Email sent successfully for message: {SequenceNumber} to {Recipient}", args.Message.SequenceNumber, recipientEmail);

            await Task.Delay(TimeSpan.FromSeconds(1));
            _logger.LogInformation("Processing complete for message: {SequenceNumber}", args.Message.SequenceNumber);
            await args.CompleteMessageAsync(args.Message);
            _logger.LogInformation("Message completed: {SequenceNumber}", args.Message.SequenceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during message processing for message: {SequenceNumber}", args.Message.SequenceNumber);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // Log the exception details
        _logger.LogError(args.Exception, "Message handler encountered an exception. ErrorSource: {ErrorSource}, EntityPath: {EntityPath}, Namespace: {FullyQualifiedNamespace}",
            args.ErrorSource,
            args.EntityPath,
            args.FullyQualifiedNamespace);

        // Note: The processor might stop processing after certain types of errors.
        // You might need logic here or in your background service to restart processing if appropriate.

        return Task.CompletedTask;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
        await _client.DisposeAsync();
    }
}
