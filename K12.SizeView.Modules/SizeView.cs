using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;

namespace K12.SizeView.Modules
{
    public partial class SizeView : BaseForm
    {
        /// <summary>
        /// FISCA Query物件
        /// </summary>
        QueryHelper _queryhelper = new QueryHelper();

        BackgroundWorker BGW = new BackgroundWorker();

        Dictionary<string, string> UDTTableName = new Dictionary<string, string>();

        Dictionary<string, string> TableNameChange = new Dictionary<string, string>();

        long TotleSize = 0;

        int SizeNow = 0;

        public SizeView()
        {
            InitializeComponent();
        }

        private void SizeView_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            timer1.Tick += new EventHandler(timer1_Tick);
            SetTableName();

            refsh();
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBarX1.Value <= SizeNow)
            {
                progressBarX1.Value++;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void SetTableName()
        {
            TableNameChange.Add("student", "學生基本資料");
            TableNameChange.Add("class", "班級基本資料");
            TableNameChange.Add("course", "課程基本資料");
            TableNameChange.Add("teacher", "教師基本資料");
            TableNameChange.Add("attendance", "缺曠記錄");
            TableNameChange.Add("discipline", "獎懲記錄");
            TableNameChange.Add("sems_moral_score", "學期德行成績");
            TableNameChange.Add("sc_attend", "學生修課記錄");
            TableNameChange.Add("exam", "試別");
            TableNameChange.Add("_udt_table", "UDT表單");
            TableNameChange.Add("sce_take", "定期評量成績");
            //定期評量成績
            TableNameChange.Add("k12.graduation.modules.allxmldataudt", "畢業封存 - XML資料");
            TableNameChange.Add("k12.graduation.modules.graduateudt", "畢業封存 - 基本資料");
            TableNameChange.Add("k12.graduation.modules.photodataudt", "畢業封存 - 照片資料");
            TableNameChange.Add("k12.graduation.modules.writteninformationudt", "畢業封存 - 書面資料");
        }

        private void refsh()
        {
            dataGridViewX1.Rows.Clear();
            progressBarX1.Value = 0;

            if (!BGW.IsBusy)
            {
                TotleSize = 0;
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中...");
            }


        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得UDT TABLE名稱
            UDTTableName.Clear();

            DataTable dt = _queryhelper.Select("SELECT * from _udt_table");
            foreach (DataRow row in dt.Rows)
            {
                UDTTableName.Add(string.Format("_$_{0}", "" + row[0]), "" + row[1]);
            }

            //取得所有Table名稱
            dt = _queryhelper.Select("select table_name from information_schema.tables where table_schema='public'");


            List<string> TableNameList = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                TableNameList.Add(string.Format("SELECT '{0}',(pg_total_relation_size('{0}'))", "" + row["table_name"]));

            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join(" union ", TableNameList) + "ORDER BY pg_total_relation_size DESC");

            dt = _queryhelper.Select(sb.ToString());

            e.Result = dt;
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                MsgBox.Show("取得資料,發生錯誤!!\n請重新開啟本畫面!!");
            }


            DataTable dt = (DataTable)e.Result;

            foreach (DataRow row in dt.Rows)
            {
                dataGridViewX1.Rows.Add(SetRow(row));
            }

            long x1 = 0;
            long x2 = 0;

            if (checkBoxX1.Checked)
            {
                x1 = TotleSize;
                x2 = 10000000000 - TotleSize;
                labelX1.Text = "資料庫目前容量：" + TotleSize.ToString() + " byte" + "　剩餘容量：" + (10000000000 - TotleSize).ToString() + " byte";
            }
            else
            {

                x1 = long.Parse((TotleSize / 1024 / 1024).ToString());
                x2 = long.Parse(((10000000000 - TotleSize) / 1024 / 1024).ToString());
                labelX1.Text = "資料庫目前容量：" + (TotleSize / 1024 / 1024) + " MB" + "　剩餘容量：" + ((10000000000 - TotleSize) / 1024 / 1024).ToString() + " MB";
            }

            double push = x1 / (x2 * 0.01);
            int push2 = 0;
            if (push.ToString().IndexOf('.') > 0)
            {
                push2 = int.Parse(push.ToString().Remove(push.ToString().IndexOf('.'))) + 1;
            }
            else
            {
                push2 = int.Parse(push.ToString());
            }

            SizeNow = push2;

            timer1.Start();
        }

        private DataGridViewRow SetRow(DataRow row)
        {
            DataGridViewRow newrow = new DataGridViewRow();
            newrow.CreateCells(dataGridViewX1);

            string name = "" + row[0];
            long size = int.Parse("" + row["pg_total_relation_size"]);

            TotleSize += size;

            if (UDTTableName.ContainsKey(name))
            {
                if (TableNameChange.ContainsKey(UDTTableName[name]))
                {
                    newrow.Cells[0].Value = name;
                    newrow.Cells[1].Value = UDTTableName[name];
                    newrow.Cells[2].Value = TableNameChange[UDTTableName[name]];

                }
                else
                {
                    newrow.Cells[0].Value = name;
                    newrow.Cells[1].Value = UDTTableName[name];

                }
            }
            else
            {
                if (TableNameChange.ContainsKey(name))
                {
                    newrow.Cells[0].Value = name;
                    newrow.Cells[2].Value = TableNameChange[name];

                }
                else
                {
                    newrow.Cells[0].Value = name;
                }

            }

            if (checkBoxX1.Checked)
            {
                newrow.Cells[colSize.Index].Value = size + " byte";
            }
            else
            {
                long tg = (size / 1024);
                string th = "";
                if (tg.ToString().Length == 6)
                    th = tg.ToString().Insert(3, ",");
                else if (tg.ToString().Length == 5)
                    th = tg.ToString().Insert(2, ",");
                else if (tg.ToString().Length == 4)
                    th = tg.ToString().Insert(1, ",");
                else
                    th = tg.ToString();
                newrow.Cells[colSize.Index].Value = th + " KB";
            }

            return newrow;

        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            refsh();
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                string a = "" + row.Cells[0].Value;
                string b = "" + row.Cells[1].Value;
                string c = "" + row.Cells[2].Value;
                if (a.Contains(textBoxX1.Text) || c.Contains(textBoxX1.Text) || b.Contains(textBoxX1.Text))
                {
                    row.Visible = true;
                }
                else
                {
                    row.Visible = false;
                }
            }
        }


    }
}
