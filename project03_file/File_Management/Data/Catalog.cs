using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Management
{
    [Serializable]

    //目录，用于存符号FCB
    //树形结构
    public class Catalog
    {
        SymFCB root_fcb ;
        
        public Catalog()
        {
            root_fcb = new SymFCB();
        }

        public SymFCB getRootFCB()
        {
            return root_fcb;
        }
    }
}
