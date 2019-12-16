using System;
using ChortkivBot.Services.DialogFlow;
using ChortkivBot.Telegram.Dialogs;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ChortkivBot.Telegram
{
    class Program
    {
        private static ITelegramBotClient botClient;
        private static DialogFlowService dialogFlowService;

        static void Main(string[] args)
        {
            dialogFlowService = new DialogFlowService();
            botClient = new TelegramBotClient("849665114:AAFHUShqVuYtW79Cf7knwbJnbF-B5OUHCow");
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Console.ReadKey();
        }
        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                var response = await dialogFlowService.SendTextAsync(e.Message.Text);
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
                switch (response.QueryResult.Intent.DisplayName)
                {
                    case "rout":
                        var routDialog = new RoutDialog(botClient);
                        routDialog.StartDialog(response, e.Message.Chat);
                        break;
                    default:
                        break;
                }
                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: response.QueryResult.FulfillmentText
                );
            }
        }
    }
}
