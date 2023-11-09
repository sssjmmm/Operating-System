using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Management
{

    [Serializable]
    //完整的FCB，符号+信息，文件控制块
    public class FCB
    {     
        SymFCB symfcb;
        MainFCB mainfcb;

        public FCB()
        {
            symfcb = null;
            mainfcb = null;
        }

        public FCB(SymFCB sf, MainFCB mf)
        {
            symfcb = sf;
            mainfcb = mf;
        }

        public SymFCB getSymFCB()
        {
            return symfcb;
        }

        public MainFCB getMainFCB()
        {
            return mainfcb;
        }
        
    }
}
