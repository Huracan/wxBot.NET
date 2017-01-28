using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wxBot.NET
{
    class TulingWXBot:wxbot
    {
        private string tuling_key = "";
        private bool robot_switch = true;
        public TulingWXBot()
        {
            try
            {
                IniHelper _IniHelper = new IniHelper(Environment.CurrentDirectory + "\\conf.ini");
                tuling_key = _IniHelper.ReadValue("main", "key");
                if (tuling_key == "")
                {
                    Console.WriteLine("[ERROR] Please set tuling key . ");
                }
                else
                {
                    Console.WriteLine("[INFO] tuling_key:" + tuling_key);
                }
            }
            catch
            {
            }
        }


        //def auto_switch(self, msg):
        //    msg_data = msg['content']['data']
        //    stop_cmd = [u'退下', u'走开', u'关闭', u'关掉', u'休息', u'滚开']
        //    start_cmd = [u'出来', u'启动', u'工作']
        //    if self.robot_switch:
        //        for i in stop_cmd:
        //            if i == msg_data:
        //                self.robot_switch = False
        //                self.send_msg_by_uid(u'[Robot]' + u'机器人已关闭！', msg['to_user_id'])
        //                #print msg['to_user_id'],self.robot_switch
        //    else:
        //        for i in start_cmd:
        //            if i == msg_data:
        //                self.robot_switch = True
        //                self.send_msg_by_uid(u'[Robot]' + u'机器人已开启！', msg['to_user_id'])
        //                #print msg['to_user_id'],self.robot_switch

        //def handle_msg_all(self, msg):
        //    if not self.robot_switch and msg['msg_type_id'] != 1:
        //        return
        //    if msg['msg_type_id'] == 1 and msg['content']['type'] == 0:  # reply to self
        //        #print msg
        //        self.auto_switch(msg)
        //    elif msg['msg_type_id'] == 4 and msg['content']['type'] == 0:  # text message from contact
        //        self.send_msg_by_uid(self.tuling_auto_reply(msg['user']['id'], msg['content']['data']), msg['user']['id'])
        //    elif msg['msg_type_id'] == 3 and msg['content']['type'] == 0:  # group text message

        public string tuling_auto_reply(string uid, wxMsg msg)
        {
            if (tuling_key != "")
            {
                string url = "http://www.tuling123.com/openapi/api";
                string user_id = uid.Replace("@", "").Substring(0, 30);
                string data = "key=" + tuling_key + "&info=" + msg.Content + "&userid=" + user_id;
                string r = Http.WebPost(url, data);
                JObject result = JsonConvert.DeserializeObject(r) as JObject;
                string rr = "";
                if (result["code"].ToString() == "100000")
                {
                    rr = result["text"].ToString().Replace("<br>", "  ");
                    rr = rr.Replace("\xa0", " ");
                }
                else if (result["code"].ToString() == "200000")
                {
                    rr = result["url"].ToString();
                }
                else if (result["code"].ToString() == "302000")
                {
                    foreach (JObject k in result["list"])
                    {
                        rr = rr + "【" + k["source"].ToString() + "】 " +
                            k["article"].ToString() + "\t" + k["detailurl"] + "\n";
                    }
                }
                else
                {
                    rr = result["text"].ToString().Replace("<br>", "  ");
                    rr = rr.Replace("\xa0", " ");
                }
                return rr + "  @Auto Reply by Turing Robot@";
            }
            else
            {
                return ("知道啦");
            }
        }


        public override void handle_msg_all(wxMsg msg)
        {
            if (!robot_switch && msg.Type != 1)
            {
                return;
            }
            if (msg.Type == 1 && msg.ContentType == 0)
            {
                auto_switch(msg);
            }
            if (msg.Type == 4 && msg.ContentType == 0)
            {
                send_msg_by_uid(tuling_auto_reply(msg.From, msg), msg.From);
            }
        }

        public void auto_switch(wxMsg msg)
        {
            string[] stop_cmd = { "退下", "走开", "关闭", "关掉", "休息", "滚开" };
            string[] start_cmd = { "出来", "启动", "工作" };
            string msg_data = msg.Content;
            if (robot_switch)
            {
                if (stop_cmd.Contains(msg_data))
                {
                    robot_switch = false;
                    send_msg_by_uid("robot off", msg.From);
                }
            }
            else
            {
                if (start_cmd.Contains(msg_data))
                {
                    robot_switch = true;
                    send_msg_by_uid("robot on", msg.From);
                }
            }
        }
    }
}
