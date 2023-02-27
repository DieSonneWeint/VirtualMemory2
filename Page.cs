using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMemory2
{
    internal class Page
    {
        public int NumberList { get; set; }
        public int Mod { get; set; }
        public DateTime Time { get; set; }
        public byte[] Data = new byte[512];
        public byte[] map = new byte[64];
        public Page(int Numblist,int mod , DateTime dateTime, byte[] masd, byte[] masb) 
        {
            NumberList = Numblist;
            Mod = mod;
            if (Mod == 0)
            {
                Time = dateTime;
            }
            Data = masd;
            map = masb;
        }
    }
}
