using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Configuration;

namespace QnABot
{
    public class BotUtility
    {
        public static T CallAPI<T>(string strUrl, string strHttpMethod, string strPostContent, List<ApiHeader> objHeaders, out HttpStatusCode code)
        {
            HttpWebRequest request = HttpWebRequest.Create(strUrl) as HttpWebRequest;
            request.Method = strHttpMethod;
            code = HttpStatusCode.OK;

            if (strPostContent != "" && strPostContent != string.Empty)
            {
                request.KeepAlive = true;
                request.ContentType = "application/json";
                for (int i=0; i< objHeaders.Count; i++)
                    request.Headers.Add(objHeaders[i].Key, objHeaders[i].Value);

                byte[] bs = Encoding.UTF8.GetBytes(strPostContent);
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(bs, 0, bs.Length);
            }

            string strReturn = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var respStream = response.GetResponseStream();
                strReturn = new StreamReader(respStream).ReadToEnd();
            }
            catch (Exception e)
            {
                strReturn = e.Message;
                code = HttpStatusCode.NotFound;
            }

            return JsonConvert.DeserializeObject<T>(strReturn);
        }

        public class ApiHeader
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}