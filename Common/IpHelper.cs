using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    public class IpHelper
    {
        /// <summary>
        /// 获取客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {
            string userIP = "";
            try
            {
                if (System.Web.HttpContext.Current == null
            || System.Web.HttpContext.Current.Request == null
            || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";
                string CustomerIP = "";
                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                return CustomerIP;
            }
            catch { }
            return userIP;
        }

        /// <summary>
        /// 调用淘宝API获取城市信息
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static string GetCity(string theip) //strIP为IP
        {
            string theurl = "http://apis.juhe.cn/ip/ipNew?ip=" + theip+ "&key=59218e436b182de1b3833d111e7c6f9a";
            string result = string.Empty;

            Result city = new Result();
            string City;
            try
            {
                result = GetRequestData(theurl);
                city = JsonConvert.DeserializeObject<Root>(result).result;
                City = city.Country + city.City;
            }
            catch
            {
                City = "本地IP";
            }
            finally
            {
            }
            return City;
        }

        public class Result
        {
            /// <summary>
            /// 
            /// </summary>
            public string Country { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Province { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string City { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Isp { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public string resultcode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string reason { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Result result { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int error_code { get; set; }
        }

        /// <summary>
        /// 获取请求数据
        /// </summary>
        /// <param name="sUrl"></param>
        /// <returns></returns>
        public static string GetRequestData(string sUrl)
        {
            //使用HttpWebRequest类的Create方法创建一个请求到uri的对象。
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sUrl);
            //指定请求的方式为Get方式
            request.Method = WebRequestMethods.Http.Get;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            //获取该请求所响应回来的资源，并强转为HttpWebResponse响应对象
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //获取该响应对象的可读流
            StreamReader reader = new StreamReader(response.GetResponseStream());
            //将流文本读取完成并赋值给str
            string str = reader.ReadToEnd();
            //关闭响应
            response.Close();
            return str;
        }
    }
}