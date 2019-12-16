using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;

namespace ChortkivBot.Services.DialogFlow
{
    public class DialogFlowService
    {
        private const string ProjectId = "chortkivbot-kltgxx";
        private const string LangCode = "uk";
        private const string SessionId = "me";

        private readonly SessionsClient client;

        public DialogFlowService()
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                @"./key.json");
            client = SessionsClient.Create();
        }

        public async Task<DetectIntentResponse> SendTextAsync(string text)
        {
            var result = await client.DetectIntentAsync(new SessionName(ProjectId, SessionId), new QueryInput
            {
                Text = new TextInput
                {
                    Text = text,
                    LanguageCode = LangCode
                }
            });

            return result;
        }
    }
}
