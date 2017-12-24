using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Configuration;

namespace QnABot.Controllers
{
    public class LineMessagesController : ApiController
    {
        string strLineChannelId = ConfigurationManager.AppSettings["LineChannelId"].ToString();
        string strLineChannelSecret = ConfigurationManager.AppSettings["LineChannelSecret"].ToString();
        string strLineMID = ConfigurationManager.AppSettings["LineMID"].ToString();

        LineBot.LineBotHelper LineBotHelper;

        public LineMessagesController()
        {
            LineBotHelper = new LineBot.LineBotHelper(strLineChannelId, strLineChannelSecret, strLineMID);
        }

        [HttpPost]
        public HttpResponseMessage Post()
        {
            //Get  Post RawData
            string postData = Request.Content.ReadAsStringAsync().Result;

            //取得LineBot接收到的訊息
            var ReceivedMessage = LineBotHelper.GetReceivedMessage(postData);

            //發送訊息
            //var ret = LineBotHelper.SendMessage(
            //    new List<string>() { ReceivedMessage.result[0].content.from },
            //        "你剛才說了 " + ReceivedMessage.result[0].content.text);

            return Request.CreateResponse(HttpStatusCode.OK);
            
            //如果給200，LineBot訊息就不會重送
            //return Request.CreateResponse(HttpStatusCode.OK, ret);
        }
    }
}
