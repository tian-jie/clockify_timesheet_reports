using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Infrastructure.Web.Domain.Service;
using System.Web.Configuration;

namespace Innocellence.WeChat.Domain.Common
{
    public class WebRequestPost
    {
        static string UserBehaviorUrl = CommonService.GetSysConfig("SSO Server", "") +
            System.Configuration.ConfigurationManager.AppSettings["UserBehaviorUrl"];

        public static string PostToUserBehavior(string oid, string actionUrl, UInt64 moduleId)
        {
            return Post(UserBehaviorUrl, string.Format("oid={0}&actionId={1}&moduleId={2}", oid, actionUrl, moduleId), Encoding.UTF8);
        }

        public static string Post(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length); //写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        public void CallUserBehavior(string functionid, string userid, string Appid, string content,string url,int contenttype)
        {
            Call(functionid, userid, Appid, content, url,contenttype);

        }

        private async Task<String> Call(string functionid, string userid, string Appid, string content,string url,int contentType)
        {
            var responseJson = string.Empty;
            try
            {


                HttpClient client = new HttpClient();
                //await client.GetAsync(url);
                //var requestJson = JsonConvert.SerializeObject(url);

                //HttpContent httpContent = new StringContent(new{url=url,oid=oid});
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("functionId", functionid));
                paramList.Add(new KeyValuePair<string, string>("userid", userid));
                paramList.Add(new KeyValuePair<string, string>("Appid", Appid));
                paramList.Add(new KeyValuePair<string, string>("Content", content));
                paramList.Add(new KeyValuePair<string, string>("url", url));
                paramList.Add(new KeyValuePair<string, string>("contentType", contentType.ToString()));
                HttpContent httpContent = new FormUrlEncodedContent(paramList);

                //httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                responseJson = client.PostAsync(UserBehaviorUrl, httpContent)
                     .Result.Content.ReadAsStringAsync().Result;

                Infrastructure.Core.Logging.LogManager.GetLogger(this.GetType()).Debug("responseJson"+ responseJson);
            }
            catch (Exception ex)
            {

            }
            return responseJson;
        }

    }
}
