using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wxBot.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            TulingWXBot newbot = new TulingWXBot();
            newbot.run();
        }
    }
}
