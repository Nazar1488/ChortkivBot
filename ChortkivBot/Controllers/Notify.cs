using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace ChortkivBot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : Controller
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly string appId;
        private readonly ConcurrentDictionary<string, ConversationReference> conversationReferences;
        private string message;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            this.adapter = adapter;
            this.conversationReferences = conversationReferences;
            appId = configuration["MicrosoftAppId"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(appId))
            {
                appId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> Send([FromForm]string message)
        {
            this.message = message;
            foreach (var conversationReference in conversationReferences.Values)
            {
                await ((BotAdapter)adapter).ContinueConversationAsync(appId, conversationReference, BotCallback, default(CancellationToken));
            }

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
        }
    }
}
