using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Configuration;
using QnABot.Models;

namespace QnABot.Controllers
{
    public class LineMessagesController : ApiController
    {
        string strLineChannelId = ConfigurationManager.AppSettings["LineChannelId"].ToString();
        string strLineChannelSecret = ConfigurationManager.AppSettings["LineChannelSecret"].ToString();
        string strLineMID = ConfigurationManager.AppSettings["LineMID"].ToString();

        [HttpPost]
        [Attributes.LineSignature]
        public IHttpActionResult Post([FromBody] LineModel.LineMessage data)
        {
            if (data == null) return BadRequest();
            if (data.events == null) return BadRequest();

            foreach (LineModel.LineMessage.Event e in data.events)
            {
                if (e.type == LineModel.LineMessage.EventType.message)
                {
                    LineModel.LineReply rb = new LineModel.LineReply()
                    {
                        replyToken = e.replyToken,
                        messages = procMessage(e.message)
                    };
                    Reply reply = new Reply(rb);
                    reply.send();

                }
            }
            return Ok(data);
        }


        private List<LineModel.SendMessage> procMessage(LineModel.ReceiveMessage m)
        {
            List<LineModel.SendMessage> objMessageList = new List<LineModel.SendMessage>();

            // 呼叫QnA Maker API，確認問的問題有沒有答案
            LineModel.SendMessage objMessage = new LineModel.SendMessage()
            {
                type = LineModel.LineMessage.MessageType.text.ToString(),
                text = QnAMaker.GetAnswer(m.text),
            };

            //switch (m.type)
            //{
            //    case LineModel.LineMessage.MessageType.sticker:
            //        sm.packageId = m.packageId;
            //        sm.stickerId = m.stickerId;
            //        break;
            //    case LineModel.LineMessage.MessageType.text:
            //        sm.text = m.text;
            //        break;
            //    default:
            //        sm.type = Enum.GetName(typeof(LineModel.LineMessage.MessageType), LineModel.LineMessage.MessageType.text);
            //        sm.text = "很抱歉，我只是一隻回音機器人，目前只能回覆基本貼圖與文字訊息喔！";
            //        break;
            //}

            objMessageList.Add(objMessage);
            return objMessageList;
        }


        public class Reply
        {
            public const string API_URL = "https://api.line.me/v2/bot/message/reply";
            private WebRequest req;

            public Reply(LineModel.LineReply body)
            {
                //--- set header and body required infos ---
                req = WebRequest.Create(API_URL);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Headers["Authorization"] = "Bearer " + ConfigurationManager.AppSettings["LineMID"].ToString();

                // --- format to json and add to request body ---
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    string data = JsonConvert.SerializeObject(body);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }
            }

            /*
                --- send message to LINE ---
                return response data
            */
            public string send()
            {
                string result = null;
                try
                {
                    WebResponse response = req.GetResponse();
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return result;
            }
        }
    }
}
