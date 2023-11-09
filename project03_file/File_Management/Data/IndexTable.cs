using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace File_Management
{
    //索引表
    [Serializable]
    public class IndexTable
    {
        //建立三级索引用于存储
        private Index firstindex;//一级索引
        private Index secondaryindex;//二级索引
        private Index thirdindex;//三级索引

        public IndexTable()
        {
            firstindex = new Index();
        }

        public bool addIndex(int data)
        {
            if (!firstindex.isfull())//一级没满
            {
                firstindex.addIndex(data);//在一级索引里加数据          
                if(firstindex.isfull() == true)//满了就开个二级
                {
                    secondaryindex = new Index();
                }
            }
            else if (!secondaryindex.isfull())//一级满但二级没满
            {
                secondaryindex.addIndex(data);//加在二级
                if (secondaryindex.isfull())//二级满就开个三级索引
                {
                    thirdindex = new Index();
                }
            }
            else if(!thirdindex.isfull())//一二级都满，三级没满加在三级
            {
               thirdindex.addIndex(data);
            }
            else//都满了，加不了
            {
                return false;
            }
            return true;//成功加上数据
        }

        public List<int> ReadTable()
        {
            //用一个列表来记录内容
            List<int> content = new List<int>();
            for(int i = 0; i < firstindex.index_num; i++)//不超过一级
            {
                content.Add(firstindex.index[i]);//把一级加到列表
            }
            if(firstindex != null && firstindex.isfull())//一级满了
            {
                for(int j = 0;j < secondaryindex.index_num; j++)//加二级到列表
                {
                    content.Add(secondaryindex.index[j]);//把二级加到列表
                }
            }
            if (secondaryindex != null && secondaryindex.isfull())//同理，二级满加三级
            {
                for(int k = 0; k <thirdindex.index_num; k++)
                {
                    content.Add(thirdindex.index[k]);
                }
            }
            return content;
        }
    }
}
