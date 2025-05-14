using Insertion_back_loss.serve;
using Insertion_back_loss.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insertion_back_loss
{
    public partial class Autotest: Form
    {
        public Autotest()
        {
            InitializeComponent();

        }
        MachinButton machinButton ;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SerialCommon serialCommon = new SerialCommon();
            serialCommon.ClosePort();
            if (checkBox1.Checked == true)
            {
                checkBox1.Text = "连续采集";
                MeasurementResult resurt1 = machinButton.read();
                label1.Text = $"模式:{resurt1.ModeName.ToString()}";
                label2.Text = $"波长:{resurt1.Wavelength.ToString()}";
                Task.Run(() =>
                {
                    double LIL = double.Parse(textBox1.Text);
                    double RIL = double.Parse(textBox2.Text);
                    double LRL = double.Parse(textBox3.Text);
                    double RRL = double.Parse(textBox4.Text);
                    double? MinIL=null;
                    double? MaxRL = null;
                    bool isIRL = false;
                    var tracker = new StateTracker<bool>();
                    int count1 = 0;
                    Dao dao = new Dao();

                    while (checkBox1.Checked == true)
                    {
                        MethodInvoker mi = new MethodInvoker(() =>
                        {
                            MeasurementResult resurt2 = machinButton.read();
                            if (LIL < resurt2.ILValue &&resurt2.ILValue < RIL && LRL < resurt2.RLValue && resurt2.RLValue < RRL)
                            {
                                if (MinIL == null) {  MinIL = resurt2.ILValue;}
                                if (MaxRL == null) { MaxRL = resurt2.RLValue; }
                                if (resurt2.ILValue < MinIL)
                                {
                                    MinIL = resurt2.ILValue;

                                }
                                if(resurt2.RLValue>MaxRL)
                                {
                                    MaxRL = resurt2.RLValue;
                                }
                                isIRL = true;
                                tracker.Update(isIRL);
                            }
                            else
                            {
                                isIRL = false;
                                tracker.Update(isIRL);
                            }
                            if (tracker.HasHistory)
                            {
                                if(tracker.Previous==true&& tracker.Current == false)
                                {

                                    string sql = $"insert into t_ILRL values('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{resurt1.ModeName}'," +
                                        $"{resurt1.Wavelength},0,0,'0',{MinIL}," +
                                        $"0,{MaxRL},0,0,0,0,0)";
                                    int n = dao.Execute(sql);
                                    if (n > 0)
                                    {
                                        count1++;
                                        label7.Text = $"第{count1}条数据保存成功";
                                        if(MinIL> double.Parse(textBox8.Text)&&MinIL< double.Parse(textBox7.Text)&&MaxRL> double.Parse(textBox6.Text)&&MaxRL< double.Parse(textBox5.Text))
                                        {
                                            label3.Text = $"IL：{MinIL.ToString()}";
                                            label3.ForeColor = Color.FromArgb(0, 255, 0);
                                            label4.Text = $"RL：{MaxRL.ToString()}";
                                            label4.ForeColor = Color.FromArgb(0, 255, 0);
                                            label12.Text = "合格";
                                            label4.ForeColor = Color.FromArgb(0, 255, 0);
                                        }
                                        else
                                        {
                                            label3.Text = $"IL：{MinIL.ToString()}";
                                            label3.ForeColor = Color.FromArgb(255, 0, 0);
                                            label4.Text = $"RL：{MaxRL.ToString()}";
                                            label4.ForeColor = Color.FromArgb(255, 0, 0);
                                            label12.Text = "不合格";
                                            label4.ForeColor = Color.FromArgb(255, 0, 0);
                                        }
                                        
                                        MinIL = null;
                                        MaxRL = null;
                                    }
                                    else
                                    {
                                        MessageBox.Show("添加失败");
                                    }
                                }
                            }
                           
                        });
                        this.BeginInvoke(mi);
                        System.Threading.Thread.Sleep(300);
                    }
                });


            }
            if (checkBox1.Checked == false)
            {
                checkBox1.Text = "停止采集";
                MeasurementResult resurt = machinButton.read();
            }
        }
    }
    public class StateTracker<T>
    {
        public T Previous { get; private set; }
        public T Current { get; private set; }

        public void Update(T newState)
        {
            Previous = Current;
            Current = newState;
        }

        public bool HasHistory => Previous != null;
    }
}
