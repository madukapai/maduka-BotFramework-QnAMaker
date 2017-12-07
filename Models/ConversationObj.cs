using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QnABot.Models;

namespace QnABot.Models
{
    public class ConversationObj
    {
        QnAMakerDbEntities db = new QnAMakerDbEntities();

        /// <summary>
        /// 取得對話資料
        /// </summary>
        /// <param name="strConversationId"></param>
        /// <returns></returns>
        public ConversationFile Query(string strConversationId)
        {
            return db.ConversationFile.Where(x => x.ConversationId == strConversationId).FirstOrDefault();
        }

        /// <summary>
        /// 增加新的對話資料
        /// </summary>
        /// <param name="strConversationId"></param>
        /// <param name="strFromName"></param>
        /// <param name="strFromId"></param>
        /// <param name="strServiceUrl"></param>
        /// <returns></returns>
        public void Add(string strConversationId, string strFromName, string strFromId, string strServiceUrl)
        {
            ConversationFile objCon = this.Query(strConversationId);

            if (objCon == null)
            {
                objCon = new ConversationFile()
                {
                    ConversationId = strConversationId,
                    CreateDate = DateTime.Now,
                    FromId = strFromId,
                    FromName = strFromName,
                    ServiceUrl = strServiceUrl,
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