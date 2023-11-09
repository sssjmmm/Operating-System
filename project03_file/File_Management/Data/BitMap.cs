using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Management
{
    //位图 用于管理磁盘中的剩余空间
    [Serializable]
    //表明该类的实例可以被序列化为二进制格式或其他可序列化的格式。
    //这使得该对象的状态可以保存到磁盘上，或者通过网络传输给其他应用程序或计算机。
    public class BitMap
    {
        public static int Capcity = 10000000;//初始化存储空间
        private Block[] blocks = new Block[Capcity];//每个块
        private bool[] bitMap = new bool[Capcity];//标志空闲,空闲为true
        private int bit_id = 0;

        public BitMap()
        {
            for (int i = 0; i < Capcity; i++)
            {
                bitMap[i] = true;//所有块设为空闲
            }
        }

        //获取第i块信息
        public string getBlock(int i)
        {
            return blocks[i].getInfo();
        }

        //找到第一个空的块
        public int allocateBlock(string data)
        {
            bit_id %= Capcity;
            int tempPointer = bit_id;
            while (true)
            {
                if (bitMap[tempPointer])
                {
                    blocks[tempPointer] = new Block();
                    blocks[tempPointer].setInfo(data);
                    bit_id = tempPointer + 1;
                    return tempPointer;
                }
                else
                {
                    tempPointer = (tempPointer + 1) % Capcity;
                }
                //遍历一整边还未找到，说明没有空的    
                if (tempPointer == bit_id)
                {
                    break;
                }
            }
            return -1;
        }

        //取出第i块，改变状态
        public void withdraw(int i)
        {
            bitMap[i] = true;
        }

        //取出多块，设置状态
        public void withdraw(List<int> indexs)
        {
            foreach(int i in indexs)
            {
                bitMap[i] = true;
            }
        }

        //建立一个索引表来保存一个文件块的数字
        public IndexTable write(string data)
        {
            IndexTable table = new IndexTable();
            while (data.Count() > 16)
            {
                table.addIndex(allocateBlock(data.Substring(0, 15)));
                data = data.Remove(0, 15);
            }
            table.addIndex(allocateBlock(data));

            return table;
        }
    }
}
