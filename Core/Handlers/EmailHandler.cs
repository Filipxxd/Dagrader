using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Core.Handlers;

public sealed class EmailHandler(IHttpClientFactory httpClientFactory, ILogger<EmailHandler> logger) : IEmailSender
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly ILogger<EmailHandler> _logger = logger;

	public string ApiKey { get; init; } = string.Empty;

	public async Task SendEmailAsync(string recipientEmail, string subject, string htmlMessage)
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient();
			httpClient.BaseAddress = new Uri("https://api.resend.com/");

			var message = new
			{
				from = "info@dagrader.xyz",
				to = recipientEmail,
				subject = subject,
				html = htmlMessage
			};

			var json = JsonSerializer.Serialize(message);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

			var response = await httpClient.PostAsync("emails", content);

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogError($"Failed to send email to {recipientEmail}. Error: {await response.Content.ReadAsStringAsync()}");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Exception occurred when sending email to {recipientEmail}");
		}
	}
}
