using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Insertion_back_loss.serve
{
    /// <summary>
    /// 设备数据解析器（支持OPM/IL/RL等模式）
    /// </summary>
    public class MeasurementResult
    {
        public byte Mode { get; set ; } // 模式
        public string ModeName { get; set; } // 模式名称
        public ushort? Wavelength { get; set; } = 0;// 波长
        public double? DisplayValue { get; set; } = 0;// 显示值
        public double? PowerValue { get; set; } = 0;// 功率值
        public byte? Unit { get; set; } = 0;// 单位
        public double? ILValue { get; set; } = 0;// 插入损耗值
        public double? RefValue { get; set; } = 0; // 参考值
        public double? RLValue { get; set; } = 0;// 回波损耗值
        public double? Length { get; set; } = 0;// 长度
        public double? IL1 { get; set; } = 0;// 第一个插入损耗值
        public double? IL2 { get; set; } = 0; // 第二个插入损耗值
        public double? RL1 { get; set; } = 0;// 第一个回波损耗值
        public double? RL2 { get; set; } = 0;// 第二个回波损耗值

    }

    public class DeviceReader
    {
        public MeasurementResult ParseResponse(byte[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (response.Length < 7)
                throw new ArgumentException("响应数据长度过短。");
            if (response[0] != 0x3C)
                throw new ArgumentException("无效的起始字节。");
            if (response[response.Length-1] != 0x3E)
                throw new ArgumentException("无效的结束字节。");

            int index = 1;
            byte id = response[index++];
            byte len = response[index++];

            // 验证长度字段
            int expectedLength = len + 2;
            if (response.Length != expectedLength)
                throw new ArgumentException("长度字段与响应数据不匹配。");

            // 验证命令字节
            if (response[index++] != 0x08)
                throw new ArgumentException("无效的命令字节。");

            // 验证响应标识
            if (response[index++] != 0x4D)
                throw new ArgumentException("无效的响应标识。");

            // 提取数据部分
            int dataLength = len - 5; 
            if (dataLength < 1)
                throw new ArgumentException("数据部分无效。");
            byte[] data = new byte[dataLength];
            Buffer.BlockCopy(response, index, data, 0, dataLength);
            index += dataLength;

            // 校验和验证
            byte expectedChecksum = response[index++];
            byte calculatedChecksum = 0;
            for (int i = 0; i < index - 1; i++)
                calculatedChecksum += response[i];
            calculatedChecksum = (byte)(~calculatedChecksum + 1); // 计算补码
            if (calculatedChecksum != expectedChecksum)
                throw new ArgumentException("校验和验证失败。");

            // 解析数据
            MeasurementResult result = new MeasurementResult { Mode = data[0] };

            switch (data[0])
            {
                case 0: // OPM模式
                    ParseOPM(data, result);
                    break;
                case 1: // IL模式
                    ParseIL(data, result);
                    break;
                case 2: // RL模式
                    ParseRL(data, result);
                    break;
                case 3: // 双波长IL
                    ParseDualIL(data, result);
                    break;
                case 4: // IL&RL
                    ParseILRL(data, result);
                    break;
                case 5: // 双波长IL&RL
                    ParseDualILRL(data, result);
                    break;
                default:
                    throw new ArgumentException("未知模式。");
            }

            return result;
        }

        private void ParseOPM(byte[] data, MeasurementResult result)
        {
            result.Wavelength = BitConverter.ToUInt16(data, 1);
            result.DisplayValue = BitConverter.ToInt32(data, 3) / 1000.0;
            result.PowerValue = BitConverter.ToInt32(data, 7) / 1000.0;
            result.Unit = data[11];
            result.ModeName = "OPM模式";
        }

        private void ParseIL(byte[] data, MeasurementResult result)
        {
            result.Wavelength = BitConverter.ToUInt16(data, 1);
            result.ILValue = BitConverter.ToInt32(data, 3) / 1000.0;
            result.RefValue = BitConverter.ToInt32(data, 7) / 1000.0;
            result.Unit = 1;
            result.ModeName = "IL模式";
        }

        private void ParseRL(byte[] data, MeasurementResult result)
        {
            result.Wavelength = BitConverter.ToUInt16(data, 1);
            result.RLValue = BitConverter.ToInt16(data, 3) / 10.0;
            result.Length = BitConverter.ToUInt16(data, 5) / 10.0;
            result.ModeName = "RL模式";
        }

        private void ParseDualIL(byte[] data, MeasurementResult result)
        {
            result.IL1 = BitConverter.ToInt32(data, 1) / 1000.0;
            result.IL2 = BitConverter.ToInt32(data, 5) / 1000.0;
            result.ModeName = "双波长IL模式";
        }

        private void ParseILRL(byte[] data, MeasurementResult result)
        {
            result.Wavelength = BitConverter.ToUInt16(data, 1);
            result.ILValue = BitConverter.ToInt32(data, 3) / 1000.0;
            result.RLValue = BitConverter.ToInt16(data, 7) / 10.0;
            result.ModeName = "IL&RL模式";
        }

        private void ParseDualILRL(byte[] data, MeasurementResult result)
        {
            result.IL1 = BitConverter.ToInt32(data, 1) / 1000.0;
            result.RL1 = BitConverter.ToInt16(data, 5) / 10.0;
            result.IL2 = BitConverter.ToInt32(data, 9) / 1000.0;
            result.RL2 = BitConverter.ToInt16(data, 13) / 10.0;
            result.ModeName = "双波长IL&RL模式";
        }

    }
    
}
