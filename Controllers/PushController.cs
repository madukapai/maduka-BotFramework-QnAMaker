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
                    var userAccount = new ChannelAccount(data[i].FromId, data[i].FromName);
                    var botAccount = new ChannelAccount(data[i].RecipientId, data[i].RecipientName);
                    var connector = new ConnectorClient(new Uri(data[i].ServiceUrl));

                    MicrosoftAppCredentials.TrustServiceUrl(data[i].ServiceUrl);

                    // 取得要送出的訊息內容物件
                    List<PushObj.GetPushFileResult> push = new PushObj().GetPushFile();

                    for (int p = 0; p < push.Count; p++)
                    {
                        IMessageActivity message = Activity.CreateMessageActivity();
                        message.ChannelId = data[i].ChannelId;
                        message.From = botAccount;
                        message.Recipient = userAccount;
                        message.Conversation = new ConversationAccount(id: data[i].ConversationId);
                        message.Locale = "zh-tw";

                        // 放入推送訊息內容
                        message.Text = push[p].MainMessage;

                        // 放入Thumbnail
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
                                    Buttons = cardButtons
                                };
                                Attachment plAttachment = plCard.ToAttachment();
                                message.Attachments.Add(plAttachment);
                            }
                        }

                        // 送出訊息
                        await connector.Conversations.SendToConversationAsync((Activity)message);
                    }
                }
            }
            catch (Exception e)
            {
                strMsg = e.Message;
            }

            return strMsg;
        }
    }
}
