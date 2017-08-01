using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wxBot.NET.Models
{
    public class wxGroup
    {
        //群名
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        //群别名
        private string _nickName;
        public string NickName
        {
            get
            {
                return _nickName;
            }
            set
            {
                _nickName = value;
            }
        }

        //群头像
        private string _headImgUrl;
        public string HeadImgUrl
        {
            get
            {
                return _headImgUrl;
            }
            set
            {
                _headImgUrl = value;
            }
        }

        //群人数
        private string _memberCount;
        public string MemberCount
        {
            get
            {
                return _memberCount;
            }
            set
            {
                _memberCount = value;
            }
        }

        //EncryChatRoomId
        private string _encryChatRoomId;
        public string EncryChatRoomId
        {
            get
            {
                return _encryChatRoomId;
            }
            set
            {
                _encryChatRoomId = value;
            }
        }

        //群内成员信息
        private List<wxGroupMember> _wxGroupMemberList=new List<wxGroupMember>();
        public List<wxGroupMember> WxGroupMemberList 
        {
            get
            {
                return _wxGroupMemberList;
            }
            set
            {
                _wxGroupMemberList = value;
            }
        }
    }
}
