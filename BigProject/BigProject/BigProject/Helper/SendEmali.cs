﻿using System.Net.Mail;
using System.Net;

namespace BigProject.Helper
{
    public class SendEmali
    {
            public string Mail { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }

            public async Task<string> SendEmailAsync(SendEmali emailTo)
            {

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tuandatnguyen3112@gmail.com", "dolm ohyj jgqj ebld"),
                    EnableSsl = true
                };
            try
                {
                    var message = new MailMessage();
                    message.From = new MailAddress("tuandatnguyen3112@gmail.com");
                    message.To.Add(emailTo.Mail);
                    message.Subject = emailTo.Subject;
                    message.Body = emailTo.Content;
                    message.IsBodyHtml = true;
                    await smtpClient.SendMailAsync(message);

                return "Gửi email thành công";
                }
                catch (Exception ex)
                {
                    return "Lỗi khi gửi email: " + ex.Message;
                }
            }
        public async Task<string> CheckEmailExistence(string email)
            {
                var client = new HttpClient();
                var apiKey = "your_api_key_here";  // Thay bằng API Key của bạn
                var url = $"https://api.hunter.io/v2/email-verifier?email={email}&api_key={apiKey}";

                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;  // Phản hồi trả về sẽ cho bạn biết email có hợp lệ hay không
            }

        }
}
