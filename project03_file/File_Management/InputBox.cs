using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Management
{
    public partial class InputBox : Form
    {
        private MainFCB textMainFCB;
        private BitMap bitMap = MainWindows.bitMap;
        public DelegateMethod.delegateFunction CallBack;

        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(MainFCB file)
        {
            InitializeComponent();
            textMainFCB = file;
            showContent();
        }

        //读取所有内容要去除尾零
        private void showContent()
        {
            List<int> indexs = textMainFCB.indextable.ReadTable();
            string content = "";
            foreach(int i in indexs)
            {
                content += bitMap.getBlock(i);
            }
            var textList = content.Split('\0');
            content = "";
            foreach (var text in textList)
            {
                content += text;
            }
            content += "\0";
            richTextBox1.Text = content;
        }

        private void FileEditor_Closing(object sender,EventArgs e)
        {
            if (MessageBox.Show("是否保存您的更改?", "提示",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                textMainFCB.modifiedTime = DateTime.Now;
                writeDisk();
                callBack();
            }
        }

        private void writeDisk()
        {
            string content = richTextBox1.Text;
            textMainFCB.size = (content.Length * 4).ToString()+"Bit";
            releaseBlock();
            textMainFCB.indextable = bitMap.write(content);
        }

        private void callBack()
        {
            if (CallBack != null)
                this.CallBack();
        }

        private void releaseBlock()
        {
            List<int> indexs = textMainFCB.indextable.ReadTable();
            bitMap.withdraw(indexs);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class DelegateMethod
    {
        public delegate void delegateFunction();
    }
}
