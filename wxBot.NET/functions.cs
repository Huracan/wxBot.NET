using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wxBot.NET
{
    class functions
    {
        public void get_icon(string uid, string gid)
        {

        }



        public string get_user_id(List<object> contact_list,string Name)
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


        //public bool send_msg_by_uid(string base_uri,string pass_ticket,,string word, string dst = "filehelper")
        //{
        //    //dst = get_user_id(dst);
        //    string url = base_uri + "/webwxsendmsg?pass_ticket=" + pass_ticket;

        //    message _message = new message();
        //    csMSG MSG = new csMSG();
        //    MSG.Type = 1;
        //    MSG.FromUserName = _me.UserName;
        //    MSG.ToUserName = dst;
        //    Random rd = new Random();
        //    double a = rd.NextDouble();
        //    string para2 = a.ToString("f3").Replace(".", string.Empty);
        //    string para1 = (DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds.ToString("f0");
        //    string msg_id = para1 + para2;
        //    word = Common.ConvertGB2312ToUTF8(word);
        //    MSG.Content = word;
        //    MSG.LocalID = msg_id;
        //    MSG.ClientMsgId = msg_id;
        //    csBaseRequest BaseRequest = new csBaseRequest();
        //    BaseRequest.Uin = uin;
        //    BaseRequest.Sid = sid;
        //    BaseRequest.Skey = skey;
        //    BaseRequest.DeviceID = device_id;

        //    _message.Msg = MSG;
        //    _message.BaseRequest = BaseRequest;

        //    string jsonStr = JsonConvert.SerializeObject(_message);
        //    string ReturnVal = Http.WebPost2(url, jsonStr);
        //    JObject jReturn = JsonConvert.DeserializeObject(ReturnVal) as JObject;
        //    return jReturn["BaseResponse"]["Ret"].ToString() == "0";

        //    //msg_id = str(int(time.time() * 1000)) + str(random.random())[:5].replace('.', '')
        //    //word = self.to_unicode(word)
        //    //    JObject sync_resul = JsonConvert.SerializeObject(sync_str) as JObject;
        //    //params = {
        //    //    'BaseRequest': self.base_request,
        //    //    'Msg': {
        //    //        "Type": 1,
        //    //        "Content": word,
        //    //        "FromUserName": self.my_account['UserName'],
        //    //        "ToUserName": dst,
        //    //        "LocalID": msg_id,
        //    //        "ClientMsgId": msg_id
        //    //    }
        //    //}
        //    //headers = {'content-type': 'application/json; charset=UTF-8'}
        //    //data = json.dumps(params, ensure_ascii=False).encode('utf8')
        //    //try:
        //    //    r = self.session.post(url, data=data, headers=headers)
        //    //except (ConnectionError, ReadTimeout):
        //    //    return False
        //    //dic = r.json()
        //    //return dic['BaseResponse']['Ret'] == 0

        //}
    }
}
