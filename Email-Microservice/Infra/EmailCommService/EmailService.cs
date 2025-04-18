using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;

namespace Email_Microservice.Infra.EmailCommService;

public class EmailService : IEmailService 
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailClient _emailClient;
        private readonly EmailConfig _emailConfig;

     
        public EmailService(IOptions<EmailConfig> emailConfig, ILogger<EmailService> logger)
        {
             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
             _emailConfig = emailConfig.Value ?? throw new ArgumentNullException(nameof(emailConfig), "EmailConfig cannot be null. Check configuration.");
            
          
             if (string.IsNullOrWhiteSpace(_emailConfig.EmailConnectionString))
             {
                 throw new ArgumentNullException(nameof(_emailConfig.EmailConnectionString), "Email ConnectionString is missing in configuration.");
             }
             _emailClient = new EmailClient(_emailConfig.EmailConnectionString);
        }

        public async Task SendEmail(string recipientEmail, string subject, string htmlContent)
        {
            var senderAddress = _emailConfig.SenderEmail;

            if (string.IsNullOrWhiteSpace(recipientEmail) || !recipientEmail.Contains('@'))
            {
                 _logger.LogError("Invalid recipient email address provided: {RecipientEmail}", recipientEmail);
                 throw new ArgumentException($"Recipient email address '{recipientEmail}' is invalid.", nameof(recipientEmail));
            }
             if (string.IsNullOrWhiteSpace(subject))
            {
                 _logger.LogWarning("Attempting to send email with empty subject to {RecipientEmail}", recipientEmail);
          
            }
             if (string.IsNullOrWhiteSpace(htmlContent))
            {
                 _logger.LogWarning("Attempting to send email with empty HTML content to {RecipientEmail}", recipientEmail);
     
            }


            _logger.LogInformation("Attempting to send email. From: {Sender}, To: {Recipient}, Subject: {Subject}", senderAddress, recipientEmail, subject);

            try
            {
                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                    WaitUntil.Started, 
                    senderAddress,     
                    recipientEmail,    
                    subject,           
                    htmlContent);      

                 _logger.LogInformation("Email send operation initiated. Operation ID: {OperationId}", emailSendOperation.Id);

            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Azure Communication Services request failed while sending email. From: {Sender}, To: {Recipient}, Subject: {Subject}. Status Code: {StatusCode}", senderAddress, recipientEmail, subject, ex.Status);
                 // Wrap in a custom exception to be caught by the message handler
                 throw new Exception($"Failed to send email via Azure Communication Services. Status: {ex.Status}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during email sending. From: {Sender}, To: {Recipient}, Subject: {Subject}", senderAddress, recipientEmail, subject);
                 // Wrap in a custom exception
                 throw new Exception("An unexpected error occurred during email sending.", ex);
            }
        }
    }