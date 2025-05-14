using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Insertion_back_loss.serve;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Insertion_back_loss.serve
{

    class MachinButton
    { 
        string portName ;
        int baudRate ;
        int dataBits ;
        byte[] data;
        StopBits stopBits ;
        Parity parity ;
        SerialCommon serialCommon = new SerialCommon();
         
        public  MachinButton(string comid) 
        { 
            portName = comid;
            baudRate = 115200;
            dataBits = 8;
            data = new byte[8];
            stopBits = StopBits.One;
            parity = Parity.None;
            serialCommon.InitComm(portName, baudRate, dataBits, stopBits, parity);
        }

        public bool con()
        {
            if (serialCommon.OpenPort())
            {
                byte[] data = new byte[] { 0x3C, 0x01, 0x05, 0x08, 0x40, 0x76, 0x3E };
                bool tx = serialCommon.SendData(data);
                byte[] rx = serialCommon.ReceiveData(1000);
                if(tx== true && rx != null)
                {
                    if (rx[0] == 0x3C && rx[1] == 0x01 && rx[2] == 0x09 && rx[3] == 0x08 && rx[4] == 0x41 && rx[5] == 0x20 && rx[6] == 0x72
                        && rx[7] == 0x30 && rx[8] == 0x08 && rx[9] == 0xA7 && rx[10] == 0x3E)
                    {
                        
                        return true;
                    }
                }

            }
            serialCommon.ClosePort();
            return  false;

        }
        public bool wavelength()
        {
            if (serialCommon.OpenPort())
            {
                byte[] data = new byte[] { 0x3C, 0x01, 0x05, 0x08, 0x22, 0x94, 0x3E };
                bool tx = serialCommon.SendData(data);
                byte[] rx = serialCommon.ReceiveData(1000);
                if (tx == true && rx != null)
                {
                    if (rx[0] == 0x3C && rx[1] == 0x01 && rx[2] == 0x05 && rx[3] == 0x08 && rx[4] == 0x23 && rx[5] == 0x93 && rx[6] == 0x3E)
                    {
                        
                        return true;
                    }
                }

            }
            serialCommon.ClosePort();
            return false;

        }
        public bool REF()
        {
            if (serialCommon.OpenPort())
            {
                byte[] data = new byte[] { 0x3C, 0x01, 0x05, 0x08, 0x24, 0x92, 0x3E };
                bool tx = serialCommon.SendData(data);
                byte[] rx = serialCommon.ReceiveData(1000);
                if (tx == true && rx != null)
                {
                    if (rx[0] == 0x3C && rx[1] == 0x01 && rx[2] == 0x05 && rx[3] == 0x08 && rx[4] == 0x25 && rx[5] == 0x91 && rx[6] == 0x3E)
                    {
                        
                        return true;
                    }
                }

            }
            serialCommon.ClosePort();
            return false;

        }
        public bool mode()
        {
            if (serialCommon.OpenPort())
            {
                byte[] data = new byte[] { 0x3C, 0x01, 0x05, 0x08, 0x26, 0x90, 0x3E };
                bool tx = serialCommon.SendData(data);
                byte[] rx = serialCommon.ReceiveData(1000);
                if (tx == true && rx != null)
                {
                    if (rx[0] == 0x3C && rx[1] == 0x01 && rx[2] == 0x05 && rx[3] == 0x08 && rx[4] == 0x27 && rx[5] == 0x8F && rx[6] == 0x3E)
                    {
                        
                        return true;
                    }
                }

            }
            serialCommon.ClosePort();
            return false;

        }
        public MeasurementResult read()
        {
            if (serialCommon.OpenPort())
            {

                byte[] data = new byte[] { 0x3C, 0x01, 0x05, 0x08, 0x4c, 0x6a, 0x3E };
                bool tx = serialCommon.SendData(data);
                byte[] rx = serialCommon.ReceiveData(1000);
                if (tx == true && rx != null)
                {
                    MeasurementResult result = new MeasurementResult();
                    DeviceReader deviceReader = new DeviceReader();
                    result = deviceReader.ParseResponse(rx);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            serialCommon.ClosePort();
            return null;
        }
        
    }
}
