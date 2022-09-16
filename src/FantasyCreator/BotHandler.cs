using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace FantasyCreator;

public class BotHandler
{
    private readonly RequestQueue Queue = new();

    public TelegramBotClient Bot { get; set; }

    public string BotToken { get; }

    public BotHandler(TelegramBotClient bot, string botToken)
    {
        Bot = bot;
        BotToken = botToken;
        Queue.OnCreateImages += OnCreateImages;
    }

    public ReceiverOptions CreateReceiverOptions()
    {
        ReceiverOptions receiverOptions1 = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        return receiverOptions1;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        string messageText = update.Message?.Text ?? update.Message?.Caption ?? "";
        string[] words = messageText.Split(" ");
        if (words.Length == 0 || update.Message == null) return;

        string text = string.Join("", messageText.SkipWhile(c => c != ' '));

        switch (words[0].ToLower())
        {
            case "/dream":
                string? imageUrl = null;
                string? maskUrl = null;

                string? fileId = update.Message?.Photo?.LastOrDefault()?.FileId;
                if (fileId != null)
                {
                    const string url = "https://api.telegram.org/file/bot{0}/{1}";
                    File file = await bot.GetFileAsync(fileId, token);
                    imageUrl = file.FilePath != null ? string.Format(url, BotToken, file.FilePath) : null;
                }

                await CreateNewRequest(text, update.Message.Chat.Id, Queue, bot, token, imageUrl, maskUrl);
                break;
            case "/order":
                await GetCurrentOrders(update.Message.Chat.Id);
                break;
        }
    }

    private async Task CreateNewRequest(
        string text, long chatId,
        RequestQueue requestQueue,
        ITelegramBotClient bot,
        CancellationToken token,
        string? imageUrl,
        string? maskUrl)
    {
        Dictionary<string, string> input = MessageUtility.ParseMessage(text);
        (bool result, string addingResponse) = requestQueue.CreateNewRequest(input, chatId, imageUrl, maskUrl);
        await bot.SendTextMessageAsync(chatId, addingResponse, cancellationToken: token);
    }

    private async Task GetCurrentOrders(long chatId)
    {
        throw new NotImplementedException();
    }

    private async void OnCreateImages(long chatId, string[] imagePaths)
    {
        if (imagePaths.Any(System.IO.File.Exists) == false)
            await Bot.SendTextMessageAsync(chatId, "error on create image");
        foreach (string path in imagePaths)
        {
            if (System.IO.File.Exists(path) == false) continue;

            FileStream stream = new(path, FileMode.Open);
            await Bot.SendPhotoAsync(chatId, new InputMedia(stream, Path.GetFileName(path)));
            stream.Close();
        }
    }
}