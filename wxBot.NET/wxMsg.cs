using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wxBot.NET
{
    public class wxMsg
    {
        /// <summary>
        /// 消息发送方
        /// </summary>
        public string From
        {
            get;
            set;
        }
        /// <summary>
        /// 消息接收方
        /// </summary>
        public string To
        {
            set;
            get;
        }
        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool Readed
        {
            get;
            set;
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg
        {
            get;
            set;
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type
        {
            get;
            set;
        }
    }
}
