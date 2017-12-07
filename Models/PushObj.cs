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
                    IsEnable = x.IsEnable,
                    MainMessage = x.MainMessage,
                    PushId = x.PushId,
                    SubTitle = x.SubTitle,
                    Title = x.Title,
                })
                .ToList();

            // 找出下面的image與button
            for (int i = 0; i < result.Count; i++)
            {
                int intPushId = result[i].PushId;
                result[i].images = db.PushImage.Where(x => x.PushId == intPushId).ToList();
                result[i].buttons = db.PushButton.Where(x => x.PushId == intPushId).ToList();
            }

            return result;
        }

        public class GetPushFileResult
        {
            public int PushId { get; set; }
            public string MainMessage { get; set; }
            public string Title { get; set; }
            public string SubTitle { get; set; }
            public bool IsEnable { get; set; }
            public List<PushImage> images { get; set; }
            public List<PushButton> buttons { get; set; }
        }
    }
}