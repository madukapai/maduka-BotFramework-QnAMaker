using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Caching;
using QnABot.Models;

namespace Microsoft.Bot.Sample.QnABot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        MemoryCache memoryCache = MemoryCache.Default;

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // 判斷是否為直接連絡真人的清單
           var strIsConnect = memoryCache.Get(activity.Conversation.Id);

            // 如果不在真人聯絡清單中，就採用自動回覆
            if (strIsConnect == null)
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog());
                }

                var reply = HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            // 把這次的對話加到資料庫中作記錄
            new ConversationObj().Add(message.Conversation.Id, "", message.ChannelId);

            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                var a = message;
                var reply = message.CreateReply("有任何簡單的問題都可以點下面的選項查看解答喔");
                reply.Type = ActivityTypes.Message;
                reply.TextFormat = TextFormatTypes.Plain;

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                    {
                        new CardAction(){ Title = "如何進行退貨", Type=ActionTypes.ImBack, Value="如何進行退貨" },
                        new CardAction(){ Title = "如何取貨", Type=ActionTypes.ImBack, Value="如何取貨" },
                        new CardAction(){ Title = "如何聯絡客服", Type=ActionTypes.ImBack, Value="如何聯絡客服" },
                        new CardAction(){ Title = "總公司的位置", Type=ActionTypes.ImBack, Value="總公司的位置" },
                        new CardAction(){ Title = "各門市的地址與資訊可以從那裡取得", Type=ActionTypes.ImBack, Value="各門市的地址與資訊可以從那裡取得" },
                        new CardAction(){ Title = "請問運費或是手續費該如何計算", Type=ActionTypes.ImBack, Value="請問運費或是手續費該如何計算" },
                        new CardAction(){ Title = "我想連絡小編", Type=ActionTypes.ImBack, Value="我想連絡小編" },
                    }
                };

                return reply;
            }
            else if (message.Type == ActivityTypes.Message)
            {
                // 如果是想跟小編講話，就先暫時把conversationId放入到快取清單中，並且不回覆任何訊息
                if (message.Text == "我想連絡小編")
                {
                    memoryCache.Set(message.Conversation.Id, "1", DateTimeOffset.UtcNow.AddDays(1));
                }
                else
                {
                    var reply = message.CreateReply("請點選下方的快速連結問題內容");
                    reply.Type = ActivityTypes.Message;
                    reply.TextFormat = TextFormatTypes.Plain;

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                    {
                        new CardAction(){ Title = "如何進行退貨", Type=ActionTypes.ImBack, Value="如何進行退貨" },
                        new CardAction(){ Title = "如何取貨", Type=ActionTypes.ImBack, Value="如何取貨" },
                        new CardAction(){ Title = "如何聯絡客服", Type=ActionTypes.ImBack, Value="如何聯絡客服" },
                        new CardAction(){ Title = "總公司的位置", Type=ActionTypes.ImBack, Value="總公司的位置" },
                        new CardAction(){ Title = "各門市的地址與資訊可以從那裡取得", Type=ActionTypes.ImBack, Value="各門市的地址與資訊可以從那裡取得" },
                        new CardAction(){ Title = "請問運費或是手續費該如何計算", Type=ActionTypes.ImBack, Value="請問運費或是手續費該如何計算" },
                        new CardAction(){ Title = "我想連絡小編", Type=ActionTypes.ImBack, Value="我想連絡小編" },
                        }
                    };

                    return reply;
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}