using Email_Microservice;
using Email_Microservice.Infra.EmailCommService;
using Email_Microservice.Infra.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

// Azure Service Bus
builder.Services.AddSingleton<IServiceBusConsumerService,ServiceBusConsumerService>();
builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBus"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailComm"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();