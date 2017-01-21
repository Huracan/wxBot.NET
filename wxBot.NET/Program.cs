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
            public override void handle_msg()
            {
                Console.WriteLine("yes");
                Console.ReadKey();
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
