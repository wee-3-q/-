using Insertion_back_loss.serve;
using Insertion_back_loss.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Insertion_back_loss
{
    public partial class Form1 : Form
    {
         MachinButton machinButton = new MachinButton("COM3");

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Table();
            
        }
        MeasurementResult resurt1 =new MeasurementResult();
        private void button4_Click(object sender, EventArgs e)
        {
            Dao dao = new Dao();
            
            string sql = $"insert into t_ILRL values('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{resurt1.ModeName}'," +
                $"{resurt1.Wavelength},{resurt1.DisplayValue},{resurt1.PowerValue},'{resurt1.Unit}',{resurt1.ILValue}," +
                $"{resurt1.RefValue},{resurt1.RLValue},{resurt1.Length},{resurt1.IL1},{resurt1.IL2},{resurt1.RL1},{resurt1.RL2 })";
            int n = dao.Execute(sql);
            if (n > 0)
            {
                MessageBox.Show("添加成功");
                Table();
            }
            else
            {
                MessageBox.Show("添加失败");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

                if (checkBox1.Checked == true)
                {
                    checkBox1.Text = "正在采集";
                Thread thread = null;
                Task.Run(() =>
                {
                    while (checkBox1.Checked == true)
                    {
                        MethodInvoker mi=new MethodInvoker(() =>
                        {
                            try
                            {
                                MeasurementResult resurt = machinButton.read();
                                string unitname = "";
                                if (resurt.Unit == 0)
                                {
                                    unitname = "dBm";
                                }
                                else if(resurt.Unit == 1)
                                {
                                    unitname = "dB";
                                }
                                    label5.Text = $"模式:{resurt.ModeName.ToString()}";
                                clear();
                                switch (resurt.Mode)
                                {
                                    case 0: // OPM模式
                                            //OPM
                                        label6.Text = $"波长:{resurt.Wavelength.ToString()}";
                                        label7.Text = $"显示：{resurt.DisplayValue.ToString()}";
                                        label8.Text = $"功率：{resurt.PowerValue.ToString()}";
                                        label9.Text = $"单位：{unitname}";
                                        break;
                                    case 1: // IL模式
                                            //IL
                                        label10.Text = $"波长:{resurt.Wavelength.ToString()}";
                                        label11.Text = $"IL：{resurt.ILValue.ToString()}";
                                        label12.Text = $"REF：{resurt.RefValue.ToString()}";
                                        label13.Text = $"单位：{resurt.RLValue.ToString()}";
                                        break;
                                    case 2: // RL模式
                                            //RL
                                        label14.Text = $"波长:{resurt.Wavelength.ToString()}";
                                        label15.Text = $"RL：{resurt.RLValue.ToString()}";
                                        label16.Text = $"长度：{resurt.Length.ToString()}";
                                        break;
                                    case 3: // 双波长IL
                                            //双波长IL
                                        label17.Text = $"IL1：{resurt.IL1.ToString()}";
                                        label18.Text = $"IL2：{resurt.IL2.ToString()}";
                                        break;
                                    case 4: // IL&RL
                                            //ILRL
                                        label2.Text = $"波长:{resurt.Wavelength.ToString()}";
                                        label3.Text = $"IL：{resurt.ILValue.ToString()}";
                                        label4.Text = $"RL：{resurt.RLValue.ToString()}";
                                        break;
                                    case 5: // 双波长IL&RL
                                            //双波长ILRL
                                        label19.Text = $"IL1：{resurt.IL1.ToString()}";
                                        label20.Text = $"IL2：{resurt.IL2.ToString()}";
                                        label21.Text = $"RL1：{resurt.RL1.ToString()}";
                                        label22.Text = $"RL2：{resurt.RL2.ToString()}";
                                        break;
                                    default:
                                        throw new ArgumentException("未知模式。");
                                }

                                
                            }
                            catch (Exception ex)
                            {
                                thread = Thread.CurrentThread;
                                thread.Abort();
                                MessageBox.Show($"串口异常{ex}");

                            }
                        });
                        this.BeginInvoke(mi);
                        System.Threading.Thread.Sleep(1000);
                    }
                });
                    

                }
                if (checkBox1.Checked == false)
                {
                    checkBox1.Text = "停止采集";
                    resurt1 = machinButton.read();
                }
        }
        public void button5_Click(object sender, EventArgs e)
        {
            MachinButton machinButton = new MachinButton(textBox1.Text);
            if (machinButton.con())
            {
                label1.Text = "连接成功";
                
            }
            else
            {
                label1.Text = "连接失败";
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!machinButton.wavelength())
            {
                MessageBox.Show("串口异常");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!machinButton.REF())
            {
                MessageBox.Show("串口异常");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!machinButton.mode())
            {
                MessageBox.Show("串口异常");
            }
        }
        public void Table()//刷新表格
        {
            dataGridView1.Rows.Clear();//清空旧数据
            Dao dao = new Dao();
            string sql = "select * from t_ILRL";
            IDataReader dc = dao.read(sql);
            while (dc.Read())
            {
                dataGridView1.Rows.Add(dc[0].ToString(), dc[1].ToString(), dc[2].ToString(), dc[3].ToString(), dc[4].ToString(),
                    dc[5].ToString(), dc[6].ToString(), dc[7].ToString(), dc[8].ToString(), dc[9].ToString(), dc[10].ToString(),
                    dc[11].ToString(), dc[12].ToString(), dc[13].ToString());

            }
            dc.Close();
            dao.DaoClose();
        }
       
        private void clear()
        {
            label6.Text = $"波长:";
            label7.Text = $"显示：";
            label8.Text = $"功率：";
            label9.Text = $"单位：";

                label10.Text = $"波长:";
                label11.Text = $"IL：";
                label12.Text = $"REF：";
                label13.Text = $"单位：";

                label14.Text = $"波长:";
                label15.Text = $"RL：";
                label16.Text = $"长度：";

                label17.Text = $"IL1：";
                label18.Text = $"IL2：";

                label2.Text = $"波长:";
                label3.Text = $"IL：";
                label4.Text = $"RL：";

                label19.Text = $"IL1：";
                label20.Text = $"IL2：";
                label21.Text = $"RL1：";
                label22.Text = $"RL2：";
          }
        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            excelserve excel = new excelserve();
            excel.ExportToExcel();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult dr = MessageBox.Show($"确认删除{time1}的数据吗？", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    string sql = $"delete from t_ILRL where time1='{time1}'";
                    Dao dao = new Dao();
                    if (dao.Execute(sql) > 0)
                    {
                        MessageBox.Show("删除成功");
                        Table();
                    }
                    else
                    {
                        MessageBox.Show("删除失败" + sql);
                        dao.DaoClose();
                    }
                }
            }
            catch
            {
                MessageBox.Show("请先在表格中选中要删除的数据", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string time1 { get; set; } //时间
        private void Getnameid()
        {
              time1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();//获取时间

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Getnameid();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Autotest autotest = new Autotest();
            autotest.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Table();
        }
    }
}
