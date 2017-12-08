using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QnABot.Models;

namespace QnABot.Models
{
    public class PushObj
    {
        QnAMakerDbEntities db = new QnAMakerDbEntities();

        /// <summary>
        /// 取得要推送的訊息物件
        /// </summary>
        /// <returns></returns>
        public List<GetPushFileResult> GetPushFile()
        {
            // 先找出主要的PushFile
            List<GetPushFileResult> result = db.PushFile
                .Where(x => x.IsEnable == true)
                .Select(x => new GetPushFileResult()
                {
                    MainMessage = x.MainMessage,
                    PushId = x.PushId,
                    PushType = x.PushType,
                })
                .ToList();

            // 找出下面的Card與button
            for (int i = 0; i < result.Count; i++)
            {
                int intPushId = result[i].PushId;

                result[i].cards = db.PushCard.Where(x => x.PushId == intPushId)
                    .Select(x => new GetPushCard()
                    {
                        ImageUrl = x.ImageUrl,
                        SubTitle = x.SubTitle,
                        Title = x.Title,
                    })
                    .ToList();

                for (int t = 0; t < result[i].cards.Count; t++)
                {
                    int indPushCardId = result[i].cards[t].PushCardId;
                    result[i].cards[t].buttons = db.PushButton.Where(x => x.PushCardId == indPushCardId).ToList();
                }
            }

            return result;
        }

        public class GetPushFileResult
        {
            public int PushId { get; set; }
            public string MainMessage { get; set; }
            public string PushType { get; set; }
            public List<GetPushCard> cards { get; set; }
        }

        public class GetPushCard
        {
            public int PushCardId { get; set; }
            public string ImageUrl { get; set; }
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public List<PushButton> buttons { get; set; }
        }
    }
}