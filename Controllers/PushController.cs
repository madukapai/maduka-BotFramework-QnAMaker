using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using QnABot.Models;

namespace QnABot.Controllers
{
    public class PushController : ApiController
    {
        /// <summary>
        /// 送出主動訊息的動作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string strConversationId)
        {
            string strMsg = "OK";

            try
            {
                List<ConversationFile> data = new Models.ConversationObj().Query(strConversationId);

                for (int i = 0; i < data.Count; i++)
                {
                    var channelId = data[i].ChannelId;

                    if (channelId != "Line")
                    {
                        await SendBotFrameworkMessage(data[i]);
                    }
                    else if (channelId == "Line")
                    {
                        SendLineMessage(data[i]);
                    }
                }
            }
            catch (Exception e)
            {
                strMsg = e.Message;
            }

            return strMsg;
        }

        private void SendLineMessage(ConversationFile data)
        {
            LineModel.LineReply rb = new LineModel.LineReply()
            {
                replyToken = data.ConversationId,
                messages = new List<LineModel.SendMessage>() { new LineModel.SendMessage() { text = "test", type="text"} },
            };
            LineMessagesController.Reply reply = new LineMessagesController.Reply(rb);
            reply.Send();
        }

        /// <summary>
        /// 送出透過Bot Framework所連接的訊息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task SendBotFrameworkMessage(ConversationFile data)
        {
            var userAccount = new ChannelAccount(data.FromId, data.FromName);
            var botAccount = new ChannelAccount(data.RecipientId, data.RecipientName);
            var connector = new ConnectorClient(new Uri(data.ServiceUrl));

            MicrosoftAppCredentials.TrustServiceUrl(data.ServiceUrl);

            // 取得要送出的訊息內容物件
            List<PushObj.GetPushFileResult> push = new PushObj().GetPushFile();

            for (int p = 0; p < push.Count; p++)
            {
                IMessageActivity message = Activity.CreateMessageActivity();
                message.ChannelId = data.ChannelId;
                message.From = botAccount;
                message.Recipient = userAccount;
                message.Conversation = new ConversationAccount(id: data.ConversationId);
                message.Locale = "zh-tw";

                // 放入推送訊息內容
                message.Text = push[p].MainMessage;

                // 放入card
                for (int t = 0; t < push[p].cards.Count; t++)
                {
                    List<CardImage> cardImages = new List<CardImage>();
                    List<CardAction> cardButtons = new List<CardAction>();

                    // 放入圖片
                    cardImages.Add(new CardImage(url: push[p].cards[t].ImageUrl));

                    // 放入Button
                    for (int b = 0; b < push[p].cards[t].buttons.Count; b++)
                    {
                        CardAction plButton = new CardAction()
                        {
                            Value = push[p].cards[t].buttons[b].Url,
                            Type = push[p].cards[t].buttons[b].ActionType,
                            Title = push[p].cards[t].buttons[b].Title,
                        };
                        cardButtons.Add(plButton);
                    }

                    if (push[p].PushType == "Thumbnail")
                    {
                        // 放入ThumbnailCard
                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = push[p].cards[t].Title,
                            Subtitle = push[p].cards[t].SubTitle,
                            Images = cardImages,
                            Buttons = cardButtons
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        message.Attachments.Add(plAttachment);
                    }
                    else if (push[p].PushType == "Hero")
                    {
                        HeroCard plCard = new HeroCard()
                        {
                            Title = push[p].cards[t].Title,
                            Subtitle = push[p].cards[t].SubTitle,
                            Images = cardImages,
                            Buttons = cardButtons,
                        };
                        Attachment plAttachment = plCard.ToAttachment();
                        message.Attachments.Add(plAttachment);
                    }

                }

                message.AttachmentLayout = push[p].Layout;

                // 送出訊息
                await connector.Conversations.SendToConversationAsync((Activity)message);
            }
        }
    }
}
