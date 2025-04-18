namespace Email_Microservice.Infra.EmailCommService;

public interface IEmailService
{
    Task SendEmail(string recipientEmail, string subject, string htmlContent);
}