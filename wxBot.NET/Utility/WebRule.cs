using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace wxBot.NET
{
    public class WebUrlRule
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public static CookieContainer CookiesContainer;

        /// <summary>  
        /// 下载网站图片  
        /// </summary>  
        /// <param name="picUrl"></param>  
        /// <returns></returns>  
        public static string SaveAsWebImg(string picUrl, string Savepath)
        {
            string result = "";
            //string path = "E:\\";  //目录  
            try
            {
                if (!String.IsNullOrEmpty(picUrl))
                {
                    Random rd = new Random();
                    DateTime nowTime = DateTime.Now;
                    string fileName = nowTime.Month.ToString() + nowTime.Day.ToString() + nowTime.Hour.ToString() + nowTime.Minute.ToString() + nowTime.Second.ToString() + rd.Next(1000, 1000000) + ".jpeg";
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(picUrl, Savepath);
                    result = fileName;
                }
            }
            catch { }
            return result;
        }


       
        public static string WebGet(string getUrl)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
                request.Method = "GET";
                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                strResult = reader.ReadToEnd();
                response.Close();
            }
            catch (Exception ex)
            {

            }

            return strResult;
        }

        public static string WebPost(string postUrl, string strPost)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(postUrl);
                Encoding encoding = Encoding.UTF8;
                //encoding.GetBytes(postData);
                byte[] bs = Encoding.ASCII.GetBytes(strPost);
                string responseData = String.Empty;
                request.Method = "POST";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie
                request.ContentLength = bs.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                strResult = reader.ReadToEnd();
                response.Close();
            }
            catch (Exception ex)
            {

            }

            return strResult;
        }

        public static string WebPost2(string postUrl, string strPost)
        {
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(postUrl);
                Encoding encoding = Encoding.UTF8;
                //encoding.GetBytes(postData);
                byte[] bs = Encoding.ASCII.GetBytes(strPost);
                string responseData = String.Empty;
                request.Method = "POST";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
                request.ContentType = "application/json; charset=UTF-8";
                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie
                request.ContentLength = bs.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                strResult = reader.ReadToEnd();
                response.Close();
            }
            catch (Exception ex)
            {

            }

            return strResult;
        }

        public static string GetKeyValue(string Url, string strKey)
        {
            string strResult = "";


            int indexofa = Url.IndexOf("?");

            string strValue = Url.Substring(indexofa + 1);
            string[] strMessage = strValue.Split('&');
            if (strMessage.Length > 0)
            {
                foreach (string strItem in strMessage)
                {

                    string[] strMessageChild = strItem.Split('=');

                    foreach (string strItemChild in strMessageChild)
                    {

                        if (strMessageChild.Length > 1 && strMessageChild[0].ToUpper().Equals(strKey.ToUpper()))
                        {
                            strResult = strMessageChild[1].ToString();
                        }

                    }
                }
            }


            return strResult;
        }
    }
}
