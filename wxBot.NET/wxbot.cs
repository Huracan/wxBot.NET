using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wxBot.NET
{
    public class wxbot
    {
        private string UNKONWN = "unkonwn";
        private string SUCCESS = "200";
        private string SCANED = "201";
        private string TIMEOUT = "408";

        private string uuid = "";
        private string redirect_uri = "";
        private string base_uri = "";
        private string base_host = "";
        private string uin = "";
        private string sid = "";
        private string skey = "";
        private string pass_ticket = "";
        private string device_id = "e"+new Random().NextDouble().ToString("f16").Replace(".", string.Empty).Substring(1);     // 'e' + repr(random.random())[2:17]
      
        private string base_request = "";
        private static Dictionary<string, string> dic_sync_key = new Dictionary<string, string>();
        private static Dictionary<string, string> dic_my_account = new Dictionary<string, string>();
        string sync_key_str = "";
        string sync_host="";
    
     
        private wxUser _me;    // 当前登录微信用户
        private List<Object> contact_list = new List<object>();   //联系人
        private List<Object> public_list = new List<object>();   //公共号
        private List<Object> special_list = new List<object>();   //特殊号
        private List<Object> group_list = new List<object>();   //群聊

        public  wxbot()
        {

        }

        /// <summary>
        /// 主逻辑
        /// </summary>
        public void run()
        {
            //获取uuid
            if (!get_uuid())
            {
                Console.WriteLine("登录失败：uuid获取失败");
            }
            //获取登录二维码
            gen_qr_code();
            Console.WriteLine("[INFO] Please use WeChat to scan the QR code .");
            //等待扫描检测
            string result = wait4login();
            if (result != SUCCESS)
            {
                Console.WriteLine("[ERROR] Web WeChat login failed. failed code=" + result);
                return;
            }
            //获取skey sid uid pass_ticket
            if (login())
            {
                Console.WriteLine("[INFO] Web WeChat login succeed .");
            }
            else
            {
                Console.WriteLine("[ERROR] Web WeChat login failed .");
                return;
            }
            //初始化
            if (init())
            {
                Console.WriteLine("[INFO] Web WeChat init succeed .");
            }
            else
            {
                Console.WriteLine("[INFO] Web WeChat init failed .");
                return;
            }
            //获取联系人
            get_contact();
            Console.WriteLine("[INFO] Get " + contact_list.Count + " contacts");
            Console.WriteLine("[INFO] Start to process messages .");            
            //开始处理消息
            proc_msg();
            Console.ReadKey();
        }

        /// <summary>
        /// 获取当前账户的所有相关账号(包括联系人、公众号、群聊、特殊账号)
        /// </summary>
        public void get_contact()
        {
            contact_list.Clear();
            public_list.Clear();
            special_list.Clear();
            group_list.Clear();

            string[] special_users = {"newsapp", "fmessage", "filehelper", "weibo", "qqmail",
                         "fmessage", "tmessage", "qmessage", "qqsync", "floatbottle",
                         "lbsapp", "shakeapp", "medianote", "qqfriend", "readerapp",
                         "blogapp", "facebookapp", "masssendapp", "meishiapp",
                         "feedsapp", "voip", "blogappweixin", "weixin", "brandsessionholder",
                         "weixinreminder", "wxid_novlwrv3lqwv11", "gh_22b87fa7cb3c",
                         "officialaccounts", "notification_messages", "wxid_novlwrv3lqwv11",
                         "gh_22b87fa7cb3c", "wxitil", "userexperience_alarm", "notification_messages"};

           string contact_str=Http.WebGet(base_uri + "/webwxgetcontact?pass_ticket=" + pass_ticket + "&skey=" + skey + "&r=" + Common.ConvertDateTimeToInt(DateTime.Now));
           JObject contact_result=JsonConvert.DeserializeObject(contact_str) as JObject;
           if (contact_result != null)
           {
               foreach (JObject contact in contact_result["MemberList"])  //完整好友名单
               {
                   wxUser user = new wxUser();
                   user.UserName = contact["UserName"].ToString();
                   user.City = contact["City"].ToString();
                   user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                   user.NickName = contact["NickName"].ToString();
                   user.Province = contact["Province"].ToString();
                   user.PYQuanPin = contact["PYQuanPin"].ToString();
                   user.RemarkName = contact["RemarkName"].ToString();
                   user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                   user.Sex = contact["Sex"].ToString();
                   user.Signature = contact["Signature"].ToString();                  
                  
                   if((int.Parse(contact["VerifyFlag"].ToString())&8)!= 0) //公众号
                   {
                       public_list.Add(user);
                   }
                   else if (special_users.Contains(contact["UserName"].ToString())) //特殊账户
                   {
                       special_list.Add(user);
                   }
                   else if (contact["UserName"].ToString().IndexOf("@@") != -1) //群聊
                   {
                       group_list.Add(user);
                   }
                   else
                   {
                       contact_list.Add(user); //联系人
                   }
               }
           }          
        }        
        /// <summary>
        /// 获取本次登录会话ID->uuid
        /// </summary>
        /// <returns></returns>
        public bool get_uuid()
        {
            string url = "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=" + Common.ConvertDateTimeToInt(DateTime.Now);
            string ReturnValue = Http.WebGet(url);
            Match match = Regex.Match(ReturnValue, "window.QRLogin.code = (\\d+); window.QRLogin.uuid = \"(\\S+?)\"");
            if (match.Success)
            {
                string code = match.Groups[1].Value;
                uuid = match.Groups[2].Value;
                return code == "200";
            }
            else
                return false;
        }
        /// <summary>
        /// 获取登录二维码
        /// </summary>
        /// <returns></returns>
        public void gen_qr_code()
        {
            string url = "https://login.weixin.qq.com/l/" + uuid;
            Image QRCode=Common.GenerateQRCode(url, Color.Black, Color.White);            
            if (QRCode != null)
            {
                QRCode.Save("img\\QRcode.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            System.Diagnostics.Process.Start("img\\QRcode.png", "rundll32.exe C://WINDOWS//system32//shimgvw.dll");            
        }
        /// <summary>
        /// 登录扫描检测
        /// </summary>
        /// <returns></returns>
        public string wait4login()
        {
            //     http comet:
            //tip=1, 等待用户扫描二维码,
            //       201: scaned
            //       408: timeout
            //tip=0, 等待用户确认登录,
            //       200: confirmed
            string tip = "1";
            int try_later_secs = 1;
            int MAX_RETRY_TIMES = 10;
            string code = UNKONWN;
            int retry_time = MAX_RETRY_TIMES;
            string status_code = null;
            string status_data = null;
            while (retry_time > 0)
            {
                string login_result = Http.WebGet("https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?" + "tip=" + tip + "&uuid=" + uuid + "&_=" + Common.ConvertDateTimeToInt(DateTime.Now));
                Match match = Regex.Match(login_result, "window.code=(\\d+)");
                if (match.Success)
                {
                    status_data = login_result;
                    status_code = match.Groups[1].Value;
                }
                if (status_code == SCANED) //已扫描 未登录
                {
                    Console.WriteLine("[INFO] Please confirm to login .");
                    tip = "0";
                }
                else if (status_code == SUCCESS)  //已扫描 已登录
                {
                    match = Regex.Match(status_data, "window.redirect_uri=\"(\\S+?)\"");
                    if (match.Success)
                    {
                        string _redirect_uri = match.Groups[1].Value + "&fun=new";
                        redirect_uri = _redirect_uri;
                        base_uri = _redirect_uri.Substring(0, _redirect_uri.LastIndexOf('/'));
                        string temp_host = base_uri.Substring(8);
                        base_host = temp_host.Substring(0, temp_host.IndexOf('/'));
                        return status_code;
                    }
                }
                else if (status_code == TIMEOUT)  //超时
                {
                    Console.WriteLine("[ERROR] WeChat login exception return_code=" + status_code + ". retry in" + try_later_secs + "secs later...");
                    tip = "1";
                    retry_time -= 1;
                    Thread.Sleep(try_later_secs * 1000);
                }
                else
                {
                    return null;
                }
                Thread.Sleep(800);
            }
            return status_code;
        }
        /// <summary>
        /// 获取skey sid uid pass_ticket  结果存放在cookies中
        /// </summary>
        public bool login()
        {
            if(redirect_uri.Length<4) 
            {
                Console.WriteLine("[ERROR] Login failed due to network problem, please try again.");
                return false;
            }
            string SessionInfo=Http.WebGet(redirect_uri);
            pass_ticket = SessionInfo.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            skey = SessionInfo.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            sid = SessionInfo.Split(new string[] { "wxsid" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            uin = SessionInfo.Split(new string[] { "wxuin" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            if(pass_ticket==""||skey==""|sid==""|uin=="")
            {
                return false;
            }
            base_request="{{\"BaseRequest\":{{\"Uin\":\"{0}\",\"Sid\":\"{1}\",\"Skey\":\"{2}\",\"DeviceID\":\"{3}\"}}}}";
            base_request = string.Format(base_request, uin, sid, skey,device_id);
            return true;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool init()
        {
            string ReturnValue = Http.WebPost(base_uri + "/webwxinit?r=" + Common.ConvertDateTimeToInt(DateTime.Now) + "&lang=en_US" + "&pass_ticket=" + pass_ticket, base_request);
            JObject init_result = JsonConvert.DeserializeObject(ReturnValue) as JObject;
            _me = new wxUser();
            _me.UserName = init_result["User"]["UserName"].ToString();
            _me.City = "";
            _me.HeadImgUrl = init_result["User"]["HeadImgUrl"].ToString();
            _me.NickName = init_result["User"]["NickName"].ToString();
            _me.Province = "";
            _me.PYQuanPin = init_result["User"]["PYQuanPin"].ToString();
            _me.RemarkName = init_result["User"]["RemarkName"].ToString();
            _me.RemarkPYQuanPin = init_result["User"]["RemarkPYQuanPin"].ToString();
            _me.Sex = init_result["User"]["Sex"].ToString();
            _me.Signature = init_result["User"]["Signature"].ToString();
            foreach (JObject synckey in init_result["SyncKey"]["List"])  //同步键值
            {
                dic_sync_key.Add(synckey["Key"].ToString(), synckey["Val"].ToString());
            }
            foreach (KeyValuePair<string, string> p in dic_sync_key)
            {
                sync_key_str += p.Key + "_" + p.Value + "%7C";
            }
            sync_key_str = sync_key_str.TrimEnd('%', '7', 'C');
            return init_result["BaseResponse"]["Ret"].ToString() =="0";
        }
        /// <summary>
        /// 状态通知
        /// </summary>
        /// <returns></returns>
        public void status_notify()
        {
            string ReturnValue = Http.WebGet(base_uri + "/webwxstatusnotify?lang=zh_CN&pass_ticket=" + pass_ticket);
           
            //return init_result["BaseResponse"]["Ret"].ToString() == "0";
        }
        /// <summary>
        /// 测试同步检查sync_host
        /// </summary>
        /// <returns></returns>
        public bool test_sync_check()
        {
            string retcode = "";
            sync_host = "webpush." + base_host;
            try
            {
                retcode = sync_check()[0];
            }
            catch
            {
                retcode = "-1";
            }
            if (retcode == "0") return true;
            //sync_host = "webpush." + base_host;
            sync_host = "webpush2." + base_host;
            try
            {
                retcode = sync_check()[0];
            }
            catch
            {
                retcode = "-1";
            }
            if (retcode == "0") return true;
            return false;
        }

        /// <summary>
        /// 同步检查
        /// </summary>
        /// <returns></returns>
       public string[] sync_check()
       {
           string retcode = "";
           string selector = "";

           string _synccheck_url = "https://{0}/cgi-bin/mmwebwx-bin/synccheck?sid={1}&uin={2}&synckey={3}&r={4}&skey={5}&deviceid={6}&_={7}";
           _synccheck_url = string.Format(_synccheck_url, sync_host, sid, uin, sync_key_str, (long)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds, skey.Replace("@", "%40"), device_id, Common.ConvertDateTimeToInt(DateTime.Now));
           try
           {
               string ReturnValue = Http.WebGet(_synccheck_url);
               Match match = Regex.Match(ReturnValue, "window.synccheck=\\{retcode:\"(\\d+)\",selector:\"(\\d+)\"\\}");
               if (match.Success)
               {
                   retcode = match.Groups[1].Value;
                   selector = match.Groups[2].Value;
               }
               return new string[2] { retcode, selector };

           }
           catch
           {
               return new string[2] { "-1", "-1" };
           }
       }

       public JObject sync()
       {
           string sync_json = "{{\"BaseRequest\" : {{\"DeviceID\":\"{6}\",\"Sid\":\"{1}\", \"Skey\":\"{5}\", \"Uin\":\"{0}\"}},\"SyncKey\" : {{\"Count\":{2},\"List\":[{3}]}},\"rr\" :{4}}}";
           string sync_keys = "";
           foreach (KeyValuePair<string, string> p in dic_sync_key)
           {
               sync_keys += "{\"Key\":" + p.Key + ",\"Val\":" + p.Value + "},";
           }
           sync_keys = sync_keys.TrimEnd(',');
           sync_json = string.Format(sync_json, uin, sid, dic_sync_key.Count, sync_keys, (long)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds, skey,device_id);

           if (sid != null && uin != null)
           {
               string sync_str = Http.WebPost(base_uri + "/webwxsync?sid=" + sid + "&lang=zh_CN&skey=" + skey + "&pass_ticket=" + pass_ticket, sync_json);
               

               JObject sync_resul = JsonConvert.DeserializeObject(sync_str) as JObject;

               if (sync_resul["SyncKey"]["Count"].ToString() != "0")
               {
                   dic_sync_key.Clear();
                   foreach (JObject key in sync_resul["SyncKey"]["List"])
                   {
                       dic_sync_key.Add(key["Key"].ToString(), key["Val"].ToString());
                   }
                   sync_key_str = "";
                   foreach (KeyValuePair<string, string> p in dic_sync_key)
                   {
                       sync_key_str += p.Key + "_" + p.Value + "%7C";
                   }
                   sync_key_str = sync_key_str.TrimEnd('%', '7', 'C');
               }
               return sync_resul;
           }
           else
           {
               return null;
           }
       }
        /// <summary>
        /// 判断用户是否在在联系人中
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
       public bool is_contact(string Name)
       {
             foreach (Object u in contact_list)
            {
                wxUser user = u as wxUser;
                if (user != null)
                {
                    if (user.UserName == Name)  
                    {
                        return true;
                    }
                }
                else
                    return false;
            }
            return false;
       }
        /// <summary>
        /// 判断用户是否是公共号
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool is_public(string Name)
       {
             foreach (Object u in public_list)
            {
                wxUser user = u as wxUser;
                if (user != null)
                {
                    if (user.UserName == Name)
                    {
                        return true;
                    }
                }
                else
                    return false;
            }
            return false;
       }
        /// <summary>
        /// 判断用户是否是特殊号
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool is_special(string Name)
        {
            foreach (Object u in special_list)
            {
                wxUser user = u as wxUser;
                if (user != null)
                {
                    if (user.UserName == Name)
                    {
                        return true;
                    }
                }
                else
                    return false;
            }
            return false;
        }

         public void handle_msg(JObject r)
         {
             //处理原始微信消息的内部函数
             //msg_type_id:
             //    0 -> Init
             //    1 -> Self
             //    2 -> FileHelper
             //    3 -> Group
             //    4 -> Contact
             //    5 -> Public
             //    6 -> Special
             //    99 -> Unknown
             //:param r: 原始微信消息
             foreach (JObject m in r["AddMsgList"])
             {
                 string from = m["FromUserName"].ToString();   //发信人ID
                 string to = m["ToUserName"].ToString();     //收信人ID
                 string content = m["Content"].ToString();
                 string content_type = m["MsgType"].ToString();
                 string MsgID=m["MsgId"].ToString();


                 wxMsg msg = new wxMsg();
            
                 msg.From = from;
                 //msg.Content = content_type == "1" ? content : "请在其他设备上查看消息";  //只接受文本消息
                 msg.Content=content;
                 msg.Readed = false;
                 msg.Time = DateTime.Now;
                 msg.To = to;
                 msg.ContentType = int.Parse(content_type);
                 msg.MsgID=MsgID;

                 if (from == _me.UserName)   // Self
                 {
                     msg.Type = 1;
                 }
                 else if (to == "filehelper")   // File Helper
                 {
                     msg.Type = 2;
                 }
                 else if (from.IndexOf("@@") != -1) //群聊
                 {
                     msg.Type = 3;
                 }
                 else if (is_contact(from))  // Contact
                 {
                     msg.Type = 4;
                 }
                 else if (is_public(from))  // Public
                 {
                     msg.Type = 5;
                 }
                 else if (is_special(from)) //special
                 {
                     msg.Type = 6;
                 }
                 else
                     msg.Type = 99;


                 if (msg.ContentType == 51)  //屏蔽一些系统数据
                 {
                     continue;
                 }
                 //foreach (Object u in contact_list)
                 //{
                 //    wxUser user = u as wxUser;
                 //    if (user != null)
                 //    {
                 //        if (user.UserName == msg.From && msg.To == _me.UserName)  //接收别人消息
                 //        {

                 //            //user.ReceiveMsg(msg);
                 //            //break;
                 //        }
                 //        else if (user.UserName == msg.To && msg.From == _me.UserName)  //同步自己在其他设备上发送的消息
                 //        {

                 //            //SendMsg(msg, true);
                 //            //break;
                 //        }
                 //    }
                 //}
                 msg=extract_msg_content(msg);
                 handle_msg_all(msg);
             }
         }

       public virtual void handle_msg_all(wxMsg msg)
       {
           //处理所有消息，请子类化后覆盖此函数
           //msg:
           //    msg_id  ->  消息id
           //    msg_type_id  ->  消息类型id
           //    user  ->  发送消息的账号id
           //    content  ->  消息内容
           //:param msg: 收到的消息
         
           //send_msg_by_uid("test,do not reply");
       }

       public class csMSG
       {
           public int Type { get; set; }
           public string Content { get; set; }
           public string FromUserName { get; set; }
           public string ToUserName { get; set; }
           public string LocalID { get; set; }
           public string ClientMsgId { get; set; }
       }

       public class csBaseRequest
       {
           public string Uin;
           public string Sid;
           public string Skey;
           public string DeviceID;           
       }

        public class message
        {
            public csMSG Msg { get; set; }
            public csBaseRequest BaseRequest { get; set; }           
        }

        public string get_user_id(string Name)
        {
            foreach (Object u in contact_list)
            {
                wxUser user = u as wxUser;
                if (user != null)
                {
                    if (user.RemarkName == Name || user.NickName == Name)  //接收别人消息
                    {
                        return user.UserName;
                    }
                }
                else
                    return "";
            }
            return "";
        }

        
         public bool  send_msg_by_uid(string word,string  dst="filehelper")
         {
             //dst = get_user_id(dst);
           string url = base_uri + "/webwxsendmsg?pass_ticket="+pass_ticket;

           message _message = new message();
           csMSG MSG = new csMSG();
           MSG.Type = 1;
           MSG.FromUserName = _me.UserName;
           MSG.ToUserName = dst;
            Random rd = new Random();
            double a = rd.NextDouble();            
            string para2=a.ToString("f3").Replace(".",string.Empty);
            string para1 = (DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds.ToString("f0");
            string msg_id = para1 + para2;
            word = Common.ConvertGB2312ToUTF8(word);
            MSG.Content = word;
            MSG.LocalID = msg_id;
            MSG.ClientMsgId = msg_id;
            csBaseRequest BaseRequest = new csBaseRequest();
            BaseRequest.Uin = uin;
            BaseRequest.Sid = sid;
            BaseRequest.Skey = skey;
            BaseRequest.DeviceID = device_id;

            _message.Msg = MSG;
            _message.BaseRequest = BaseRequest;

            string jsonStr = JsonConvert.SerializeObject(_message);
            string ReturnVal=Http.WebPost2(url, jsonStr);
            JObject jReturn=JsonConvert.DeserializeObject(ReturnVal) as JObject;
            return jReturn ["BaseResponse"]["Ret"].ToString() == "0";
          
        //msg_id = str(int(time.time() * 1000)) + str(random.random())[:5].replace('.', '')
        //word = self.to_unicode(word)
        //    JObject sync_resul = JsonConvert.SerializeObject(sync_str) as JObject;
        //params = {
        //    'BaseRequest': self.base_request,
        //    'Msg': {
        //        "Type": 1,
        //        "Content": word,
        //        "FromUserName": self.my_account['UserName'],
        //        "ToUserName": dst,
        //        "LocalID": msg_id,
        //        "ClientMsgId": msg_id
        //    }
        //}
        //headers = {'content-type': 'application/json; charset=UTF-8'}
        //data = json.dumps(params, ensure_ascii=False).encode('utf8')
        //try:
        //    r = self.session.post(url, data=data, headers=headers)
        //except (ConnectionError, ReadTimeout):
        //    return False
        //dic = r.json()
        //return dic['BaseResponse']['Ret'] == 0

        }




        public  virtual void schedule()
        {
        //做任务型事情的函数，如果需要，可以在子类中覆盖此函数
        //此函数在处理消息的间隙被调用，请不要长时间阻塞此函数
        } 
       

        /// <summary>
        /// 处理消息
        /// </summary>
        public void proc_msg()
        {
            test_sync_check();
            while (true)
            {
                float check_time = (float)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds;
                try
                {
                    string[] ReturnArray = sync_check();//[retcode, selector] 
                    string retcode = ReturnArray[0];
                    string selector = ReturnArray[1];

                    if (retcode == "1100")  //从微信客户端上登出
                        break;
                    else if (retcode == "1101") // 从其它设备上登了网页微信
                        break;
                    else if (retcode == "0")
                    {
                        if (selector == "2")  // 有新消息
                        {
                            JObject r = sync();
                            if (r != null)
                            {
                                handle_msg(r);
                            }
                        }
                        //else if ( selector == "3")  // 未知
                        //{
                        //    r = self.sync()
                        //    if r is not None:
                        //        self.handle_msg(r)
                        //}
                        else if (selector == "4")   //通讯录更新
                        {
                            JObject r = sync();
                            if (r != null)
                            {
                                get_contact();
                                Console.WriteLine("[INFO] Contacts Updated .");
                            }
                        }
                        else
                        {
                        }
                        //    r = self.sync()
                        //    if r is not None:
                        //        self.get_contact()
                        //elif selector == '6':  # 可能是红包
                        //    r = self.sync()
                        //    if r is not None:
                        //        self.handle_msg(r)
                        //elif selector == '7':  # 在手机上操作了微信
                        //    r = self.sync()
                        //    if r is not None:
                        //        self.handle_msg(r)
                        //elif selector == '0':  # 无事件
                        //    pass
                        //else:
                        //    print '[DEBUG] sync_check:', retcode, selector
                        //    r = self.sync()
                        //    if r is not None:
                        //        self.handle_msg(r)
                    }
                    else
                    {
                        Thread.Sleep(2000);
                        schedule();                        
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("[ERROR] Except in proc_msg");
                    Console.WriteLine(ex.ToString());
                }
                check_time = (float)(DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds - check_time;
                if (check_time < 0.8)
                    Thread.Sleep((int)(1.0 - check_time) * 1000);
            }
        }
        /// <summary>
        /// 解析收到的消息
        /// </summary>
        /// <param name="msg"></param>    
        ///  content_type_id:
        ///    0 -> Text
        ///    1 -> Location
        ///    3 -> Image
        ///    4 -> Voice
        ///    5 -> Recommend
        ///    6 -> Animation
        ///    7 -> Share
        ///    8 -> Video
        ///    9 -> VideoCall
        ///    10 -> Redraw
        ///    11 -> Empty
        ///    99 -> Unknown
        ///:param msg_type_id: 消息类型id
        ///:param msg: 消息结构体
        ///:return: 解析的消息
        public wxMsg extract_msg_content(wxMsg msg)
        {
            int mtype = msg.ContentType;
            //string content = HTMLParser.HTMLParser().unescape(msg['Content']);
            string content = System.Net.WebUtility.HtmlDecode(msg.Content);

            string msg_id = msg.MsgID;

            //msg_content = {}
            if (msg.Type == 0)
            {
                msg.ContentType = 11;
            }
            else if (msg.Type == 2)     //File Helper
            {
                msg.ContentType = 0;
            }
            else if (msg.Type == 3)   // 群聊
            {
                int sp = content.IndexOf("<br/>");
                string uid = content.Substring(0, sp);
                content = content.Substring(sp);
                content = content.Replace("<br/>", "");
                //uid = uid.Remove(uid.Length-1, 1);
                //name = self.get_contact_prefer_name(self.get_contact_name(uid))
                //if not name:
                //    name = self.get_group_member_prefer_name(self.get_group_member_name(msg['FromUserName'], uid))
                //if not name:
                //    name = 'unknown'
                //msg_content['user'] = {'id': uid, 'name': name}
            }



            else//# Self, Contact, Special, Public, Unknown
            {
            }

            //msg_prefix = (msg_content['user']['name'] + ':') if 'user' in msg_content else ''

            if (mtype == 1)
            {
                if (content.IndexOf("http://weixin.qq.com/cgi-bin/redirectforward?args=") != -1)
                {
                    //string r =Http.WebGet(content);
                    //r.encoding = 'gbk'
                    //data = r.text
                    //pos = self.search_content('title', data, 'xml')
                    //msg_content['type'] = 1
                    //msg_content['data'] = pos
                    //msg_content['detail'] = data
                    //if self.DEBUG:
                    //    print '    %s[Location] %s ' % (msg_prefix, pos)
                }
                else
                {
                    msg.ContentType = 0;
                    //if msg_type_id == 3 or (msg_type_id == 1 and msg['ToUserName'][:2] == '@@'):  # Group text message
                    //    msg_infos = self.proc_at_info(content)
                    //    str_msg_all = msg_infos[0]
                    //    str_msg = msg_infos[1]
                    //    detail = msg_infos[2]
                    //    msg_content['data'] = str_msg_all
                    //    msg_content['detail'] = detail
                    //    msg_content['desc'] = str_msg
                    //else:
                    //    msg_content['data'] = content
                    //if self.DEBUG:
                    //    try:
                    //        print '    %s[Text] %s' % (msg_prefix, msg_content['data'])
                    //    except UnicodeEncodeError:
                    //        print '    %s[Text] (illegal text).' % msg_prefix
                }
            }
            //elif mtype == 3:
            //    msg_content['type'] = 3
            //    msg_content['data'] = self.get_msg_img_url(msg_id)
            //    msg_content['img'] = self.session.get(msg_content['data']).content.encode('hex')
            //    if self.DEBUG:
            //        image = self.get_msg_img(msg_id)
            //        print '    %s[Image] %s' % (msg_prefix, image)
            //elif mtype == 34:
            //    msg_content['type'] = 4
            //    msg_content['data'] = self.get_voice_url(msg_id)
            //    msg_content['voice'] = self.session.get(msg_content['data']).content.encode('hex')
            //    if self.DEBUG:
            //        voice = self.get_voice(msg_id)
            //        print '    %s[Voice] %s' % (msg_prefix, voice)
            //elif mtype == 37:
            //    msg_content['type'] = 37
            //    msg_content['data'] = msg['RecommendInfo']
            //    if self.DEBUG:
            //        print '    %s[useradd] %s' % (msg_prefix,msg['RecommendInfo']['NickName'])
            //elif mtype == 42:
            //    msg_content['type'] = 5
            //    info = msg['RecommendInfo']
            //    msg_content['data'] = {'nickname': info['NickName'],
            //                           'alias': info['Alias'],
            //                           'province': info['Province'],
            //                           'city': info['City'],
            //                           'gender': ['unknown', 'male', 'female'][info['Sex']]}
            //    if self.DEBUG:
            //        print '    %s[Recommend]' % msg_prefix
            //        print '    -----------------------------'
            //        print '    | NickName: %s' % info['NickName']
            //        print '    | Alias: %s' % info['Alias']
            //        print '    | Local: %s %s' % (info['Province'], info['City'])
            //        print '    | Gender: %s' % ['unknown', 'male', 'female'][info['Sex']]
            //        print '    -----------------------------'
            //elif mtype == 47:
            //    msg_content['type'] = 6
            //    msg_content['data'] = self.search_content('cdnurl', content)
            //    if self.DEBUG:
            //        print '    %s[Animation] %s' % (msg_prefix, msg_content['data'])
            //elif mtype == 49:
            //    msg_content['type'] = 7
            //    if msg['AppMsgType'] == 3:
            //        app_msg_type = 'music'
            //    elif msg['AppMsgType'] == 5:
            //        app_msg_type = 'link'
            //    elif msg['AppMsgType'] == 7:
            //        app_msg_type = 'weibo'
            //    else:
            //        app_msg_type = 'unknown'
            //    msg_content['data'] = {'type': app_msg_type,
            //                           'title': msg['FileName'],
            //                           'desc': self.search_content('des', content, 'xml'),
            //                           'url': msg['Url'],
            //                           'from': self.search_content('appname', content, 'xml'),
            //                           'content': msg.get('Content')  # 有的公众号会发一次性3 4条链接一个大图,如果只url那只能获取第一条,content里面有所有的链接
            //                           }
            //    if self.DEBUG:
            //        print '    %s[Share] %s' % (msg_prefix, app_msg_type)
            //        print '    --------------------------'
            //        print '    | title: %s' % msg['FileName']
            //        print '    | desc: %s' % self.search_content('des', content, 'xml')
            //        print '    | link: %s' % msg['Url']
            //        print '    | from: %s' % self.search_content('appname', content, 'xml')
            //        print '    | content: %s' % (msg.get('content')[:20] if msg.get('content') else "unknown")
            //        print '    --------------------------'

            //elif mtype == 62:
            //    msg_content['type'] = 8
            //    msg_content['data'] = content
            //    if self.DEBUG:
            //        print '    %s[Video] Please check on mobiles' % msg_prefix
            //elif mtype == 53:
            //    msg_content['type'] = 9
            //    msg_content['data'] = content
            //    if self.DEBUG:
            //        print '    %s[Video Call]' % msg_prefix
            //elif mtype == 10002:
            //    msg_content['type'] = 10
            //    msg_content['data'] = content
            //    if self.DEBUG:
            //        print '    %s[Redraw]' % msg_prefix
            //elif mtype == 10000:  # unknown, maybe red packet, or group invite
            //    msg_content['type'] = 12
            //    msg_content['data'] = msg['Content']
            //    if self.DEBUG:
            //        print '    [Unknown]'
            //else:
            //    msg_content['type'] = 99
            //    msg_content['data'] = content
            //    if self.DEBUG:
            //        print '    %s[Unknown]' % msg_prefix
            //return msg_content
            return msg;
        }
        
    }
}
