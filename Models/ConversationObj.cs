using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QnABot.Models;
using Microsoft.Bot.Connector;

namespace QnABot.Models
{
    public class ConversationObj
    {
        QnAMakerDbEntities db = new QnAMakerDbEntities();

        /// <summary>
        /// 回傳所有的談話記錄
        /// </summary>
        /// <returns></returns>
        public List<ConversationFile> Query()
        {
            return db.ConversationFile.ToList();
        }

        /// <summary>
        /// 取得對話資料
        /// </summary>
        /// <param name="strConversationId"></param>
        /// <returns></returns>
        public List<ConversationFile> Query(string strConversationId)
        {
            return db.ConversationFile.Where(x => x.ConversationId == strConversationId).ToList();
        }

        /// <summary>
        /// 增加新的對話資料
        /// </summary>
        /// <param name="strConversationId"></param>
        /// <param name="strFromName"></param>
        /// <param name="strFromId"></param>
        /// <param name="strServiceUrl"></param>
        /// <returns></returns>
        public void Add(Activity message)
        {
            ConversationFile objCon = this.Query(message.Conversation.Id).FirstOrDefault();

            if (objCon == null)
            {
                objCon = new ConversationFile()
                {
                    ConversationId = message.Conversation.Id,
                    CreateDate = DateTime.Now,
                    FromId = message.From.Id,
                    FromName = message.From.Name,
                    ServiceUrl = message.ServiceUrl,
                    ChannelId = message.ChannelId,
                    RecipientId = message.Recipient.Id,
                    RecipientName = message.Recipient.Name,
                };

                db.ConversationFile.Add(objCon);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 加入Line的對話用戶到交談資料表中
        /// </summary>
        /// <param name="strReplyToken"></param>
        /// <param name="strUserId"></param>
        public void AddLine(string strReplyToken, string strUserId)
        {
            ConversationFile objCon = this.Query(strReplyToken).FirstOrDefault();

            if (objCon == null)
            {
                objCon = new ConversationFile()
                {
                    ConversationId = strReplyToken,
                    CreateDate = DateTime.Now,
                    FromId = strUserId,
                    FromName = strUserId,
                    ServiceUrl = "https://api.line.me/v2/bot/message/reply",
                    ChannelId = "Line",
                    RecipientId = "",
                    RecipientName = "",
                };

                db.ConversationFile.Add(objCon);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 刪除現有的對話資料
        /// </summary>
        /// <param name="strConversationId"></param>
        /// <returns></returns>
        public void Delete(string strConversationId)
        {
            db.ConversationFile.RemoveRange(db.ConversationFile.Where(x => x.ConversationId == strConversationId).ToList());
        }
    }
}