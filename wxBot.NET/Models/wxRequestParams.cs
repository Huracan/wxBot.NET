using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wxBot.NET.Models
{
    public class wxRequestParams
    {
        #region batch_get_group_members
        public class BaseRequestParam
        {
            public string Sid;
            public string Skey;
            public string DeviceID;
            public string Uin;
        }

        public class listParam
        {
            public string UserName;
            public string EncryChatRoomId;
        }

        public class GroupPostParams
        {
            public int Count;
            public BaseRequestParam BaseRequest;
            public List<listParam> List = new List<listParam>();
        }
        #endregion
    }
}
