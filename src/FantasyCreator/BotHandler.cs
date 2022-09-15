using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace FantasyCreator;

public class BotHandler
{
    private readonly RequestQueue Queue = new();

    public TelegramBotClient Bot { get; set; }

    public BotHandler(TelegramBotClient bot)
    {
        Bot = bot;
        Queue.OnCreateImages += OnCreateImages;
    }

    public ReceiverOptions CreateReceiverOptions()
    {
        List<UpdateType> allowedUpdates = Enum.GetValues(typeof(UpdateType)).Cast<UpdateType>().ToList();
        allowedUpdates.Remove(UpdateType.Message);
        ReceiverOptions receiverOptions1 = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() //allowedUpdates.ToArray()
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
        string messageText = update.Message?.Text ?? "";
        string[] words = messageText.Split(" ");
        if (words.Length == 0 || update.Message == null) return;

        string text = string.Join("", messageText.SkipWhile(c => c != ' '));

        switch (words[0].ToLower())
        {
            case "!dream":
            case "/dream":
            case "/дрим":
            case "!дрим":
                await CreateNewRequest(text, update.Message.Chat.Id, Queue, bot, token);
                break;
            case "!очередь":
            case "/очередь":
            case "!order":
            case "/order":
                await GetCurrentOrders(update.Message.Chat.Id);
                break;
        }
    }

    private async Task CreateNewRequest(
        string text, long chatId,
        RequestQueue requestQueue,
        ITelegramBotClient bot,
        CancellationToken token)
    {
        Dictionary<string, string> input = MessageUtility.ParseMessage(text);
        (bool result, string addingResponse) = requestQueue.CreateNewRequest(input, chatId);
        await bot.SendTextMessageAsync(chatId, addingResponse, cancellationToken: token);
    }

    private async Task GetCurrentOrders(long chatId)
    {
        throw new NotImplementedException();
    }

    private async void OnCreateImages(long chatId, string[] imagePaths)
    {
        if (imagePaths.Any(File.Exists) == false)
        {
            await Bot.SendTextMessageAsync(chatId, "error on create image");
        }
        foreach (string path in imagePaths)
        {
            if(File.Exists(path) == false) continue;

            FileStream stream = new(path, FileMode.Open);
            await Bot.SendPhotoAsync(chatId, new InputMedia(stream, Path.GetFileName(path)));
            stream.Close();
        }
    }
}