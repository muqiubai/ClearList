using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearList
{
    public partial class UnityProject : Form
    {
        //文件属性会包含的字段
        string field = "RecentlyUsedProjectPaths";

        //注册表固定的卸载选项路径 - Unity
        string registryPath = @"Software\Unity Technologies\Unity Editor 5.x";

        string errMessage = "";

        private DataTable tableData;

        private List<string> lists;

        Dictionary<string, string> receiveLists = new Dictionary<string, string>();


        public UnityProject()
        {
            InitializeComponent();

            tableData = new DataTable();

            lists = new List<string>();
        }



        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnityProject_Load(object sender, EventArgs e)
        {
            tableData.Clear();

            AddDataGridViewColumn();

            AddCheckBoxItem();

            DefaultAcquisition();
        }


        /// <summary>
        /// 选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectBtn_Click(object sender, EventArgs e)
        {
            OnBtnClickChoice();
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            OnBtnClickDelete();
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            DefaultAcquisition();
        }

        /// <summary>
        /// 默认获取
        /// </summary>
        private void DefaultAcquisition()
        {
            receiveLists.Clear();

            tableData.Clear();

            receiveLists = RegistryData.GetUnityProjectList(registryPath, field, ref errMessage);

            Dictionary<string, string>.Enumerator dic = receiveLists.GetEnumerator();

            while (dic.MoveNext())
            {
                //需要截取一下 Value 的最后一个字段
                int index = dic.Current.Value.LastIndexOf('/');
                string fileName = dic.Current.Value.Substring(index + 1);

                AddDataGridViewRow(fileName, dic.Current.Value, dic.Current.Key);
            }

            DataGridView.DataSource = tableData;

            ViewPropertySettings();
        }

        /// <summary>
        /// 添加列
        /// </summary>
        private void AddDataGridViewColumn()
        {
            tableData.Columns.Add("项目名称", typeof(string));
            tableData.Columns.Add("项目路径", typeof(string));
            tableData.Columns.Add("注册表项名称", typeof(string));

            DataGridView.AllowUserToAddRows = false;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void AddDataGridViewRow(string _projectName, string _filePath, string _tableName)
        {
            DataRow dr = tableData.NewRow();
            
            dr["项目名称"] = _projectName;
            dr["项目路径"] = _filePath;
            dr["注册表项名称"] = _tableName;

            tableData.Rows.Add(dr);
        }

        /// <summary>
        /// 添加ChechBox列
        /// </summary>
        private void AddCheckBoxItem()
        {
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "选择";
            checkBoxColumn.Name = "Select";
            checkBoxColumn.TrueValue = true;
            checkBoxColumn.FalseValue = false;
            checkBoxColumn.DataPropertyName = "IsChecked";
            //在第一列插入
            DataGridView.Columns.Insert(0, checkBoxColumn);
        }

        /// <summary>
        /// 属性设置
        /// </summary>
        private void ViewPropertySettings()
        {
            for (int i = 0; i < DataGridView.Columns.Count; i++)
            {
                //禁止点击 列标题进行排序
                DataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //除了第一列，其他列禁止编辑
            for (int i = 1; i < DataGridView.Columns.Count; i++)
            {
                DataGridView.Columns[i].ReadOnly = true;
            }

            //设定列的最小宽度
            DataGridView.Columns[0].Width = 50;
            DataGridView.Columns[0].Resizable = DataGridViewTriState.False;
            DataGridView.Columns[1].MinimumWidth = 150;
            DataGridView.Columns[2].MinimumWidth = 350;
            DataGridView.Columns[3].MinimumWidth = 325;
        }

        /// <summary>
        /// 点击选择
        /// </summary>
        private void OnBtnClickChoice()
        {
            //退出编辑状态
            DataGridView.EndEdit();

            if (SelectBtn.Text == "全选")
            {
                SelectBtn.Text = "反选";
                SelectBtn.Image = global::ClearList.Properties.Resources.Choice_Normal;

                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    DataGridView.Rows[i].Cells["Select"].Value = true;
                }
            }
            else
            {
                SelectBtn.Text = "全选";
                SelectBtn.Image = global::ClearList.Properties.Resources.Choice_Selected;

                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    DataGridView.Rows[i].Cells["Select"].Value = false;
                }
            }
        }

        /// <summary>
        /// 点击删除
        /// </summary>
        private void OnBtnClickDelete()
        {
            DataGridView.EndEdit();

            lists.Clear();

            for (int i = 0; i < DataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = DataGridView.Rows[i];

                if (row.Cells[0].Value != null)
                {
                    if (bool.Parse(row.Cells[0].Value.ToString()) == true)
                    {
                        //记录改行信息，并进行删除
                        lists.Add(row.Cells[3].Value.ToString());
                        DataGridView.Rows.Remove(row);
                        i--;
                    }
                }
            }

            //调用注册表删除方法
            RegistryData.DeleteRegist(registryPath, lists.ToArray(), ref errMessage);

            //刷新一下组件
            

            if (!string.IsNullOrEmpty(errMessage))
            {
                MessageBox.Show(errMessage);
            }
        }
    }
}
