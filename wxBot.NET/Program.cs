using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wxBot.NET
{
    class Program
    {
        class newbot : wxbot
        {
            public override void handle_msg_all(wxMsg msg)
            {
                if (msg.Type == 4 && msg.ContentType==0)
                {
                    string uid = get_user_id("Teano");
                    send_msg_by_uid("test,do not reply", uid);
                }
            }
        }

        static void Main(string[] args)
        {
          
            newbot _newbot = new newbot();
            //_newbot.handle_msg();
            _newbot.run();
            
        }
    }
}
