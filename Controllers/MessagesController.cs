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
using QnABot;

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
            Activity reply = null;

            // 如果不在真人聯絡清單中，就找出問題的內容
            if (strIsConnect == null)
            {
                if (activity.GetActivityType() == ActivityTypes.Message)
                {
                    QuestionObj Question = new QuestionObj();

                    // 找出是否有答案
                    QuestionFile answer = Question.GetQuestionAnswer(activity.Text);

                    if (answer == null)
                    {
                        // 找不到問題，也找不到答案，後送QnA
                        // await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog());
                        string strAnswer = QnAMaker.GetAnswer(activity.Text);
                        reply = this.ReplyOptions(activity, strAnswer, Question.GetQuestionCards(0));
                    }
                    else
                    {
                        List<CardAction> cards = new List<CardAction>();

                        // 如果是回到主選單，就取出根目錄的內容
                        if (activity.Text == "<-回主選單")
                        {
                            cards = Question.GetQuestionCards(0);
                        }
                        else
                        {
                            cards = Question.GetNextQuestionCards(activity.Text);
                        }

                        // 找到問題，判斷是否有下一層，有下一層就產生選單
                        if (cards.Count > 0)
                        {
                            reply = this.ReplyOptions(activity, null, cards);
                        }
                        else
                        {
                            // 沒有下一層的話，就判斷回覆的內容是否往QnA送，並顯示相同層級的問題清單
                            // 一般問題就直接回覆
                            if (answer.AnswerType == "QnA")
                            {
                                await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog());

                                // 我想聯絡小編就不發回覆
                                if (activity.Text != "我想連絡小編")
                                    reply = this.ReplyOptions(activity, null, Question.GetSameLevelQuestion(activity.Text));
                            }
                            else
                                reply = this.ReplyOptions(activity, answer.Answer, Question.GetSameLevelQuestion(activity.Text));
                        }
                    }
                }

                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            // 把這次的對話加到資料庫中作記錄
            new ConversationObj().Add(message);

            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                this.ReplyOptions(message, "點下面的選項查看解答喔", new QuestionObj().GetQuestionCards(0));
            }
            else if (message.Type == ActivityTypes.Message)
            {
                // 如果是想跟小編講話，就先暫時把conversationId放入到快取清單中，並且不回覆任何訊息
                if (message.Text == "我想連絡小編")
                    memoryCache.Set(message.Conversation.Id, "99", DateTimeOffset.UtcNow.AddDays(1));
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

        /// <summary>
        /// 回送指定的問題項目
        /// </summary>
        /// <param name="message">訊息物件</param>
        /// <param name="strMessage">主要訊息內容</param>
        /// <param name="cards">回送的卡片物件</param>
        /// <returns></returns>
        private Activity ReplyOptions(Activity message, string strMessage, List<CardAction> cards)
        {
            var reply = message.CreateReply();

            if (!string.IsNullOrEmpty(strMessage))
                reply.Text = strMessage;
            else
                reply.Text = "請點選下方選單";

            reply.Type = ActivityTypes.Message;
            reply.TextFormat = TextFormatTypes.Plain;

            if (cards != null)
                reply.SuggestedActions = new SuggestedActions() { Actions = cards };

            return reply;
        }
    }
}