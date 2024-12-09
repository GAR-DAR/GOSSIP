using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net.IO
{
    public class Packet<T>
    {
        public T Data { get; set; }
    }
}
