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
        public async Task<string> Get()
        {
            string strMsg = "OK";

            try
            {
                List<ConversationFile> data = new Models.ConversationObj().Query();

                for (int i = 0; i < data.Count; i++)
                {
                    var userAccount = new ChannelAccount(data[i].FromId, data[i].FromName);
                    var botAccount = new ChannelAccount(data[i].RecipientId, data[i].RecipientName);
                    var connector = new ConnectorClient(new Uri(data[i].ServiceUrl));

                    IMessageActivity message = Activity.CreateMessageActivity();
                    message.ChannelId = data[i].ChannelId;
                    message.From = botAccount;
                    message.Recipient = userAccount;
                    message.Conversation = new ConversationAccount(id: data[i].ConversationId);
                    message.Text = "這是QnA Maker的主動訊息";
                    message.Locale = "zh-tw";
                    await connector.Conversations.SendToConversationAsync((Activity)message);
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
