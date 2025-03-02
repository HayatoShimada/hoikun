namespace hoikun.Services
{
    using Azure;
    using Azure.Communication.Email;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderEmail; // 送信元アドレス

        public EmailService(IConfiguration configuration)
        {
            string? connectionString = configuration["AzureCommunicationServices:ConnectionString"];
            _senderEmail = configuration["AzureCommunicationServices:SenderEmail"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(_senderEmail))
            {
                throw new InvalidOperationException("Azure Communication Services の設定が不足しています。");
            }

            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                EmailContent emailContent = new(subject)
                {
                    PlainText = body
                };

                // 修正：EmailAddress型に変換
                EmailRecipients recipients = new(new List<EmailAddress> { new(recipientEmail) });

                EmailMessage emailMessage = new(_senderEmail, recipients, emailContent);

                EmailSendOperation sendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                Console.WriteLine($"Email sent successfully to {recipientEmail}. Message ID: {sendOperation.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        public async Task SendEmailsAsync(List<string> recipientEmails, string subject, string body)
        {
            try
            {
                EmailContent emailContent = new(subject)
                {
                    PlainText = body
                };

                // List<string> → List<EmailAddress> に変換
                EmailRecipients recipients = new(recipientEmails.Select(email => new EmailAddress(email)).ToList());

                EmailMessage emailMessage = new(_senderEmail, recipients, emailContent);

                EmailSendOperation sendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                Console.WriteLine($"Emails sent successfully. Message ID: {sendOperation.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }


    }

}
