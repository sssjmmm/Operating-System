using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Management
{
    [Serializable]
    //同样要序列化
    //物理存储块
    public class Block
    {
        private char[] info;//块的信息
        private int len;//所占磁盘的长度

        public Block()
        {
            info = new char[16];
            len = 0;
         }

        //设置信息
        public void setInfo(string info_str)
        {
            len = (info_str.Length > 16) ? 16 : info_str.Length;
            for(int i = 0; i < len; ++i)
            {
                info[i] = info_str[i];
            }
        }

        //获取块信息
        public string getInfo()
        {
            string temp = new string(info);
            return temp;
        }
    }
}
