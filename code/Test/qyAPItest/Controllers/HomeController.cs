//using Innocellence.WeChat.Domain.Entity;
//using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace qyAPITest.Controllers
{
    public class HomeController : WechatQYBaseController
    {

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Test(string TestInterface)
        {
            var para = Request.Form;
            var keys = string.Join(",", Request.Form.AllKeys);

            var appid = WebConfigurationManager.AppSettings["AppID"];


            List<KeyValuePair<string, string>> a = new List<KeyValuePair<string, string>>();
            foreach (var b in para.AllKeys)
            {
                a.Add(new KeyValuePair<string, string>(b, para[b]));
            }

            var lst = a.ToList();

            lst.Add(new KeyValuePair<string, string>("appId", appid));
            lst.Add(new KeyValuePair<string, string>("timestamp", GetTimestamp().ToString()));
            lst.Add(new KeyValuePair<string, string>("nonce", GetNoncestr()));
            lst.Add(new KeyValuePair<string, string>("sign", GetSignature(keys + ",appId,timestamp,nonce", lst)));

            var str = await new FormUrlEncodedContent(lst).ReadAsStringAsync();



           var APIUrl = WebConfigurationManager.AppSettings["APIUrl"];
           var strRet= GetHtmlFromUrl(APIUrl + TestInterface, str);




           return Content(strRet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="strEncoding"></param>
        /// <returns></returns>st
        public static string GetHtmlFromUrl(string Url, string strData, string strEncoding = "UTF-8")
        {

            try
            {


                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byte1 = Encoding.UTF8.GetBytes(strData);

                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                wReq.Method = "Post";
                wReq.ContentType = "application/x-www-form-urlencoded";

                System.IO.Stream newStream = wReq.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);
                newStream.Close();


                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();




                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(strEncoding)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetSignature(string param, List<KeyValuePair<string, string>> RequestPara)
        {
            //获取所有的参数
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var _params = param.Split(',');

            foreach (var a in _params)
            {
                dict.Add(a, RequestPara.Find(b => b.Key == a).Value);
            }


            // var config = GetWeChatConfigByID(int.Parse(Request["AppID"]));
            //Todo
            // dict.Add("appSignKey", "iMih0xabKQdw8CBbkTM5Ley84WhN4oL6u5lbDui6G9tUlQo7fJE1CcktZ2UiETnU1FZ0R3ZvzYLKOzmaziyms5QuMia8czkEwFv2TQUg4G45Ha0aHPEHXnhjVqUPnKPJ");

            dict.Add("appSignKey", "R2xBNTNTVmxocTRIbm45V0lKeURZaW93RkpnQTIzUWV3OCtzejllL29ObXdUdVYwSWtXT0dnPT0=");

            StringBuilder sb = new StringBuilder();
            //参数排序
            var keys = dict.Keys.OrderBy(a => a);

            foreach (var a in keys)
            {
                sb.AppendFormat("&{0}={1}", a, dict[a]);
            }

            //加密
            return EncryptHelper.GetSha1(sb.ToString().Substring(1)).ToLower();
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            Random random = new Random();
            return EncryptHelper.GetMD5(random.Next(1000).ToString(), "GBK");
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

    }
}