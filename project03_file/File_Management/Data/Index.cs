using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Management
{
    [Serializable]
    //索引
    class Index
    {
        public int[] index;//索引序列
        public int index_num;//方便增删索引，指向最后一位
        public Index()
        {
            index = new int[100];
            index_num = 0;
        }
        public void addIndex(int data)
        {
            index[index_num] = data;//数据放最后一位
            index_num++;//num++
        }
        public bool isfull()//判满
        {
            return (index_num >= 100);
        }
    }
}
