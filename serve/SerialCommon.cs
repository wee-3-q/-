using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Insertion_back_loss.serve
{
    class SerialCommon
    {
        public string[] portName = SerialPort.GetPortNames();//获取当前计算机的串行端口名称数组
        public SerialPort MySerialPort = new SerialPort();

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="PortName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        public bool InitComm(string PortName, int BaudRate, int DataBits, StopBits stopBits, Parity parity)
        {
            try
            {
                if (MySerialPort.IsOpen)
                {
                    MySerialPort.Close();
                }
                MySerialPort.PortName = PortName;
                MySerialPort.BaudRate = BaudRate;
                MySerialPort.DataBits = DataBits;
                MySerialPort.StopBits = stopBits;
                MySerialPort.Parity = parity;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool OpenPort()
        {
            try
            {
                if (!MySerialPort.IsOpen)
                {
                    MySerialPort.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"串口打开异常{ex}");
                return false;
            }
            return true;
        }
        public bool ClosePort()
        {
            try
            {
                if (MySerialPort != null)
                    MySerialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"串口关闭异常{ex}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public bool SendData(byte[] data)
        {
            if (MySerialPort.IsOpen)
            {
                try
                {
                    MySerialPort.DiscardInBuffer();
                    MySerialPort.DiscardOutBuffer();
                    MySerialPort.Write(data, 0, data.Length);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"串口数据发送异常{ex}");
                }
            }
            return false;
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="_Timeout"></param>
        /// <returns></returns>
        public byte[] ReceiveData(int _Timeout)
        {
            byte[] buffer = new byte[] { };
            DateTime t = DateTime.Now;
            try
            {
                #region 接收到数据中断 或 在规定时间内无任何数据中断
                while (true)
                {
                    if (MySerialPort.BytesToRead > 0) { break; }
                    if (DateTime.Compare(DateTime.Now, t.AddMilliseconds(_Timeout)) > 0) { break; }
                    Thread.Sleep(5);
                }
                #endregion
                while (true)
                {
                    Thread.Sleep(50);
                    int n = MySerialPort.BytesToRead;
                    byte[] buf = new byte[n];
                    if (n < 1) { break; }//无数据中断退出
                    MySerialPort.Read(buf, 0, n);
                    buffer = UniteArray(buffer, buf);
                    if (DateTime.Compare(DateTime.Now, t.AddMilliseconds(_Timeout)) > 0)
                    {
                        break;
                    }
                }
                return buffer;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"串口数据接收异常{ex}");
                return buffer;
            }
        }
        ///合并数组
        public byte[] UniteArray(byte[] ArrayMain, byte[] ArrayItme)
        {
            if (ArrayMain == null) { return ArrayItme; }
            if (ArrayItme == null) { return ArrayMain; }
            byte[] val = new byte[ArrayMain.Length + ArrayItme.Length];
            for (int i = 0; i < val.Length; i++)
            {
                if (i < ArrayMain.Length)
                {
                    val[i] = ArrayMain[i];
                }
                else
                {
                    val[i] = ArrayItme[i - ArrayMain.Length];
                }
            }
            return val;
        }
    }
}
