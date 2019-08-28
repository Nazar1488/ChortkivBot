using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChortkivBot.Contracts.Services;
using ChortkivBot.Core.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Location;
using Microsoft.Bot.Schema;

namespace ChortkivBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly IRoutService routeService;

        public EchoBot(IRoutService routeService)
        {
            this.routeService = routeService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Activity reply;
            List<CardAction> actions;
            switch (turnContext.Activity.Text)
            {
                case RoutPhrases.Routes:
                    var routes = await routeService.GetAvailableRoutes();
                    reply = MessageFactory.Text("Наявні маршрути:");
                    actions = new List<CardAction>();
                    foreach (var route in routes)
                    {
                        var action = new CardAction
                        {
                            Title = route.Names[0],
                            Text = RoutPhrases.Stops,
                            Type = ActionTypes.MessageBack,
                            Value = route.Id,
                            DisplayText = route.Names[0]
                        };
                        actions.Add(action);
                    }

                    reply.SuggestedActions = new SuggestedActions
                    {
                        Actions = actions
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;
                case RoutPhrases.Stops:
                    var rout = await routeService.GetRoutById((long)turnContext.Activity.Value);
                    reply = MessageFactory.Text($"Зупинки по маршруту {rout.Names[0]}");
                    actions = new List<CardAction>();
                    foreach (var stop in rout.Stops)
                    {
                        var action = new CardAction
                        {
                            Title = stop.Names[0],
                            Text = RoutPhrases.BusInfo,
                            Type = ActionTypes.MessageBack,
                            Value = stop.Id,
                            DisplayText = stop.Names[0]
                        };
                        actions.Add(action);
                    }

                    reply.SuggestedActions = new SuggestedActions
                    {
                        Actions = actions
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;
                case RoutPhrases.BusInfo:
                    var stops = await routeService.GetStopInfo((long) turnContext.Activity.Value);
                    var message = new StringBuilder();
                    foreach (var stopInfo in stops)
                    {
                        var bus = await routeService.GetBusById(stopInfo.RoutId, stopInfo.BusId);
                        message.AppendLine($"Маршрутка {bus.Number} прибуде через {stopInfo.Time} хв");
                    }
                    reply = MessageFactory.Text(message.ToString());
                    reply.SuggestedActions = new SuggestedActions
                    {
                        Actions = new List<CardAction>
                        {
                            new CardAction { Title = "Маршрутки", Type = ActionTypes.MessageBack, Text = RoutPhrases.Routes, DisplayText = "Маршрутки" },
                        }
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;
                default:
                    reply = MessageFactory.Text("Привіт! Я бот міста Чорткова :) Чим можу допомогти?");
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>
                        {
                            new CardAction { Title = "Маршрутки", Type = ActionTypes.MessageBack, Text = RoutPhrases.Routes, DisplayText = "Маршрутки" },
                        },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text("Привіт! Я бот міста Чорткова :) Чим можу допомогти?");
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>
                        {
                            new CardAction { Title = "Маршрутки", Type = ActionTypes.MessageBack, Text = RoutPhrases.Routes, DisplayText = "Маршрутки" },
                        },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}
