using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace File_Management
{
    public partial class MainWindows : Form
    {
        public SymFCB SymFCBnow;//此时选择的FCB
        Stack<SymFCB> FCBpath = new Stack<SymFCB>();//用栈来记录文件路径
        Catalog catalog = new Catalog();//用目录来存symFCB
        public Dictionary<int, FCB> pairtable = new Dictionary<int, FCB>(); //符号与FCB对
        public static BitMap bitMap = new BitMap();//位图（管理空闲区域）
        public string curPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory()));//当前路径
        private Dictionary<int, ListViewItem> list_table = new Dictionary<int, ListViewItem>();//文件显示框
        TreeNode root;//文件树的根结点

        public MainWindows()
        {
            InitializeComponent();
            SymFCBnow = catalog.getRootFCB();
            FCBpath.Push(SymFCBnow);
            InitializeWindows();
        }
        
        public void InitializeWindows()
        {
            InitializeListView();
            InitializeTreeView();
            SymFCBnow = catalog.getRootFCB();
            textBox1.Text = "root\\";
        }

        public void map(SymFCB sfcb, MainFCB mfcb)
        {
            FCB fcb = new FCB(sfcb, mfcb);
            pairtable[sfcb.file_id] = fcb;
        }

        //展示更新
        public void UpdateView()
        {
            UpdateTreeView();
            UpdateListView(SymFCBnow);
        }

        //初始化列表
        public void InitializeListView()
        {
            listView1.Items.Clear();//列表清空
        }

        //初始化目录树
        public void InitializeTreeView()
        {
            treeView1.Nodes.Clear();//目录树清空
            root = new TreeNode("root");//添加根节点
            treeView1.Nodes.Add(root);
            treeView1.ExpandAll();
        }

        //更新目录树
        public void UpdateTreeView()
        {
            treeView1.Nodes.Clear();
            root = new TreeNode("root");
            DFStree(root,catalog.getRootFCB());
            treeView1.Nodes.Add(root);
            treeView1.ExpandAll();
        }

        //更新列表
        public void UpdateListView(SymFCB item)
        {
            list_table = new Dictionary<int, ListViewItem>();
            listView1.Items.Clear();
            if(item.son != null)
            {
                SymFCB son = item.son;
                do
                {
                    MainFCB temp = pairtable[son.file_id].getMainFCB();
                    ListViewItem file = new ListViewItem(new string[]
                    {
                        temp.name,
                        temp.size,
                        temp.type,
                        temp.modifiedTime.ToString()
                    });
                    if (temp.type == "folder")
                        file.ImageIndex = 0;
                    else
                        file.ImageIndex = 1;

                    listMap(temp, file);
                    listView1.Items.Add(file);
                    son = son.next;
                } while (son != null);
            }
        }

        //DFS构造树
        private void DFStree(TreeNode node,SymFCB curFCB)
        {
            if(curFCB.son != null)
            {
                SymFCB son = curFCB.son;
                do
                {
                    if (son.fileType == SymFCB.FileType.folder)
                    {
                        TreeNode new_node = new TreeNode(son.fileName);
                        DFStree(new_node, son);
                        node.Nodes.Add(new_node);
                    }
                    else if(son.fileType == SymFCB.FileType.txt)
                    {
                        TreeNode new_node = new TreeNode(son.fileName);
                        new_node.ImageIndex = new_node.SelectedImageIndex =  1;
                        node.Nodes.Add(new_node);
                    }
                    son = son.next;
                } while(son != null);
            }
        }

        //判断同一目录下名字是否重复，并生成新的文件名
        private string Checkname(string s,string ext = "")
        {
            SymFCB current_curPath = SymFCBnow.son;
            int counter = 0;
            int max_folder = 0;
            int max_file = 0;
            int num;
            while(current_curPath != null)
            {
                string[] sArray = current_curPath.fileName.Split('(');
                string[] sArray2 = current_curPath.fileName.Split(')');
                char c = sArray2[0][sArray2[0].Length - 1];
                if (char.IsDigit(c))
                {
                    num = c - '0';
                    if(sArray[0][0] == 'f')
                    {
                        max_folder = max_folder > num ? max_folder : num;
                    }
                    else if (sArray[0][0] == 't')
                    {
                        max_file = max_file > num ? max_file : num;
                    }
                }
               
                if (sArray[0] != current_curPath.fileName && sArray[0] == s) 
                {
                    counter++;
                    
                }
                else if (current_curPath.fileName == s + ext)
                {
                    counter++;
                }
                current_curPath = current_curPath.next;
            
                //MessageBox.Show(sArray2[0][sArray2[0].Length - 1].ToString ());

            }
            //MessageBox.Show(counter.ToString());
            //MessageBox.Show(max_folder.ToString());

            if(counter > 0 && s[0] == 'f')
                counter = counter > (max_folder+1) ? counter : (max_folder+1);
            if (counter > 0 && s[0] == 't')
                counter = counter > (max_file + 1) ? counter : (max_file + 1);
            if (counter > 0)
                s += "(" + counter.ToString() + ")";

            return s + ext;
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0 )
            {
               current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请先选择对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();

            openClick(current_fcb, current_file);
        }

        //判断打开的是文件还是文件夹
        private void openClick(SymFCB fcb,MainFCB file)
        {
            switch (fcb.fileType){
                case SymFCB.FileType.folder:
                    SymFCBnow = fcb;
                    FCBpath.Push(fcb);
                    textBox1.Text = pairtable[SymFCBnow.file_id].getMainFCB().path;
                    UpdateListView(fcb);
                    break;
                case SymFCB.FileType.txt:
                    InputBox fileEditor = new InputBox(file);
                    fileEditor.Show();
                    fileEditor.CallBack = UpdateView;
                    break;
                default:
                    break;
            }
        }

        //新建文件
        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_name = Checkname("text", ".txt");
            string fatherPath;

            SymFCB new_fcb = new SymFCB(file_name, SymFCB.FileType.txt);//新建symFCB
            SymFCBnow.addSonItem(new_fcb);

            MainFCB father = null;
            if (pairtable.ContainsKey(SymFCBnow.file_id))
            {
                father = pairtable[SymFCBnow.file_id].getMainFCB();
            }
            fatherPath = (father == null) ? "root" : father.path;
            MainFCB new_file = new MainFCB(new_fcb, fatherPath);//新建MainFCB
            map(new_fcb, new_file);

            UpdateView();//更新视图
        }

        //新建文件夹
        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_name = Checkname("folder");
            string fatherPath;
            
            SymFCB new_fcb = new SymFCB(file_name, SymFCB.FileType.folder);//添加新的 SymFCB
            SymFCBnow.addSonItem(new_fcb);

            MainFCB father = null;
            if (pairtable.ContainsKey(SymFCBnow.file_id))
            {
                father = pairtable[SymFCBnow.file_id].getMainFCB();
            }
            fatherPath = (father == null) ? "root" : father.path;
            MainFCB new_file = new MainFCB(new_fcb, fatherPath);//添加新的 MainFCB
            map(new_fcb, new_file);

            UpdateView();
        }

        //删除功能
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem current_item;
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请先选择对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();
            //获取文件块
            List<int> indexs = current_file.indextable.ReadTable();
            //位图删除物理块
            bitMap.withdraw(indexs);
            //删除目录中的symFCB
            current_fcb.remove();
            //删除FCB
           pairtable.Remove(current_fcb.file_id);

            UpdateView();
        }

        
        public void listMap(MainFCB file, ListViewItem item)
        {
            list_table[file.file_id] = item;
        }
                
        public int getPointer(ListViewItem item)
        {
            if (list_table.ContainsValue(item))
            {
                foreach (KeyValuePair<int, ListViewItem> kvp in list_table)
                {
                    if (kvp.Value.Equals(item))
                        return kvp.Key;
                }
                return -1;
            }
            else
            {
                MessageBox.Show("无法获取地址信息");
                return -1;
            }
        }


        //文件列表双击
        private void listView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请先选择对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();

            openClick(current_fcb, current_file);
        }

        //重命名
        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请先选择对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();

            WriteName renameBox = new WriteName(current_file,current_fcb);
            renameBox.Show();
            renameBox.CallBack = UpdateView;

        }

        //保存文件
        public void saveData()
        {
            FileStream fileStream1, fileStream2, fileStream3;
            BinaryFormatter a = new BinaryFormatter();

            fileStream1 = new FileStream(System.IO.Path.Combine(curPath, "pairTable.dat"), FileMode.Create);
            a.Serialize(fileStream1, pairtable);
            fileStream1.Close();

            fileStream2 = new FileStream(System.IO.Path.Combine(curPath, "catalogTable.dat"), FileMode.Create);
            a.Serialize(fileStream2, catalog);
            fileStream2.Close();

            fileStream3 = new FileStream(System.IO.Path.Combine(curPath, "bitMap.dat"), FileMode.Create);
            a.Serialize(fileStream3, bitMap);
            fileStream3.Close();

            string filePath = "file_counter.txt"; // 文件路径
            // 创建一个新的StreamWriter对象，将数据写入文件
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(SymFCB.file_counter);
                //MessageBox.Show(SymFCB.file_counter.ToString(), "Tips");
            }

            MessageBox.Show("历史数据已保存" , "Tips");
        }

        //加载文件
        public void loadData()
        {
            FileStream fileStream1, fileStream2, fileStream3;
            BinaryFormatter b = new BinaryFormatter();

            fileStream1 = new FileStream(System.IO.Path.Combine(curPath, "pairTable.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            pairtable = b.Deserialize(fileStream1) as Dictionary<int, FCB>;
            fileStream1.Close();

            fileStream2 = new FileStream(System.IO.Path.Combine(curPath, "catalogTable.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            catalog = b.Deserialize(fileStream2) as Catalog;
            fileStream2.Close();

            fileStream3 = new FileStream(System.IO.Path.Combine(curPath, "bitMap.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            bitMap = b.Deserialize(fileStream3) as BitMap;
            fileStream3.Close();

            string filePath = "file_counter.txt"; // 文件路径

            // 检查文件是否存在
            if (File.Exists(filePath))
            {
                // 创建一个新的StreamReader对象，读取文件内容
                using (StreamReader reader = new StreamReader(filePath))
                {
                    int data = int.Parse(reader.ReadToEnd());
                    //MessageBox.Show(data.ToString(), "Tips");
                    SymFCB.file_counter = data;
                }
            }
            else
            {
                //Console.WriteLine("文件不存在。");
            }

            InitializeWindows();
            UpdateView();
            MessageBox.Show("文件已加载完毕", "Tips");
        }

        //关闭窗口
        public void MainWindows_Closing(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否保存?", "Tips", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveData();
            }
        }

        
        //返回功能
        private void button1_Click(object sender, EventArgs e)
        {
            if (SymFCBnow.file_id == catalog.getRootFCB().file_id)
                return;
            SymFCBnow = SymFCBnow.father;
            if (SymFCBnow.file_id == catalog.getRootFCB().file_id)
                textBox1.Text = "root\\";
            else
                textBox1.Text = pairtable[SymFCBnow.file_id].getMainFCB().path;
            UpdateListView(SymFCBnow);
        }


        //格式化功能
        private void 格式化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pairtable = new Dictionary<int, FCB>();
            catalog = new Catalog();
            FCBpath = new Stack<SymFCB>();
            bitMap = new BitMap();
            SymFCBnow = catalog.getRootFCB();
            UpdateView();
            InitializeWindows();
        }

        //项目信息
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("选择控件1进行载入与保存\n" +
                "选择控件2对文件/文件夹进行相应操作\n" +
                "单击左侧目录树可查看相应目录下的文件\n" +
                "在文件框选择对象后单击右键可进行打开/删除/重命名", "使用说明", MessageBoxButtons.OK);
        }

        //加载之前数据
        private void 载入历史文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadData();
            UpdateView();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveData();
        }

        //右击属性
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        //重命名1
        private void 重命名ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请选择一个对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();

            WriteName renameBox = new WriteName(current_file, current_fcb);
            renameBox.Show();
            renameBox.CallBack = UpdateView;

        }


        //删除功能1
        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem current_item;
            //先选择一个对象
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请选择一个对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();
            //获取文件块并从位图中删除
            List<int> indexs = current_file.indextable.ReadTable();
            bitMap.withdraw(indexs);
            //删除符号目录项树中的符号目录项
            current_fcb.remove();
            //删除目录中的FCB
            pairtable.Remove(current_fcb.file_id);

            UpdateView();
        }

        //打开
        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请选择一个对象");
                return;
            }

            MainFCB current_file = pairtable[getPointer(current_item)].getMainFCB();
            SymFCB current_fcb = pairtable[current_file.file_id].getSymFCB();

            openClick(current_fcb, current_file);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
