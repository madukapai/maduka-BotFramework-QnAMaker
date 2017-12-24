using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QnABot.Models;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;

namespace QnABot
{
    public class QnAMaker
    {
        static string strUrl = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{0}/generateAnswer";

        /// <summary>
        /// 透過QnA Maker API取得回答
        /// </summary>
        /// <param name="strQuestion"></param>
        /// <returns></returns>
        public static string GetAnswer(string strQuestion)
        {
            string strAnswer = "";
            strUrl = string.Format(strUrl, ConfigurationManager.AppSettings["QnAKnowledgebaseId"].ToString());
            HttpStatusCode code = HttpStatusCode.OK;

            List<BotUtility.ApiHeader> objHeaders = new List<BotUtility.ApiHeader>()
            {
                new BotUtility.ApiHeader(){
                    Key = "Ocp-Apim-Subscription-Key",
                    Value = ConfigurationManager.AppSettings["QnASubscriptionKey"].ToString()
                }
            };

            QnaMakerModel.QnAMakerQuery query = new QnaMakerModel.QnAMakerQuery() { question = strQuestion, };
            QnaMakerModel.QnAMakerResult result = BotUtility.CallAPI<QnaMakerModel.QnAMakerResult>(strUrl, "POST", JsonConvert.SerializeObject(query), objHeaders, out code);

            if (result.answers.Count > 0)
            {
                if (result.answers[0].score > 0)
                    strAnswer = result.answers[0].answer;
            }

            if (string.IsNullOrEmpty(strAnswer))
                strAnswer = "找不到相關的回答，您可以嘗試使用其他的問題或是關鍵字詢問";

            return strAnswer;
        }
    }
}