using FantasyCreator;
using Telegram.Bot;

const string botTokenName = "TELEGRAM_BOT_TOKEN";
string? botToken = Environment.GetEnvironmentVariable(botTokenName);
if (botToken == null) throw new Exception($"Bot token not exist, please use {botTokenName} environment variable");

using CancellationTokenSource cts = new();
TelegramBotClient bot = new(botToken);

BotHandler botHandler = new BotHandler(bot);
bot.StartReceiving(
    botHandler.HandleUpdateAsync,
    botHandler.HandlePollingErrorAsync,
    botHandler.CreateReceiverOptions(),
    cts.Token
);

Console.ReadLine();
await bot.CloseAsync();