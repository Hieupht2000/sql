using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CarManagetment.Services.Notification
{
    public class TelegramService
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;

        public TelegramService(IConfiguration configuration)
        {
            _botToken = configuration["Telegram:BotToken"];
            _chatId = configuration["Telegram:ChatId"];
            _httpClient = new HttpClient();
        }

        public async Task SendMessageAsync(string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={_chatId}&text={Uri.EscapeDataString(message)}";
            var data = new { chat_id = _chatId, text = message };
            var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(url, content);

        }
    }
}
