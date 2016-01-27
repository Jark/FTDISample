using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Perception;
using Windows.Devices.SerialCommunication;
using FTDI.D2xx.WinRT.Device;

namespace FTDISample.Serial
{
    public class FtdiSerialDevice : ISerialDevice
    {
        private readonly IFTDevice ftDevice;

        public FtdiSerialDevice(IFTDevice ftDevice)
        {
            this.ftDevice = ftDevice;
        }

        public async Task SetConnectionSettings(DeviceConnection.ConnectionSettings connectionSettings)
        {
            await ftDevice.SetBaudRateAsync(connectionSettings.BaudRate);
            await ftDevice.SetDataCharacteristicsAsync(GetWordLength(connectionSettings.DataBits), GetStopBits(connectionSettings.StopBits), GetParity(connectionSettings.Parity));
            await ftDevice.SetFlowControlAsync(GetFlowControl(connectionSettings.Handshake), connectionSettings.XOn, connectionSettings.XOff);
        }

        private static WORD_LENGTH GetWordLength(ushort dataBits)
        {
            switch (dataBits)
            {
                case 7:
                    return WORD_LENGTH.BITS_7;
                case 8:
                    return WORD_LENGTH.BITS_8;
                default:
                    throw new Exception("Only data bits of 7 or 8 is supported for the FTDIDevice.");
            }
        }

        private static STOP_BITS GetStopBits(SerialStopBitCount stopBits)
        {
            switch (stopBits)
            {
                case SerialStopBitCount.One:
                    return STOP_BITS.BITS_1;
                case SerialStopBitCount.Two:
                    return STOP_BITS.BITS_2;
                default:
                    throw new Exception($"SerialStopBitCount {stopBits} is not supported for the FTDIDevice.");
            }
        }

        private static PARITY GetParity(SerialParity parity)
        {
            switch (parity)
            {
                case SerialParity.Even:
                    return PARITY.EVEN;
                    case SerialParity.Mark:
                    return PARITY.MARK;
                    case SerialParity.None:
                    return PARITY.NONE;
                    case SerialParity.Odd:
                    return PARITY.ODD;
                case SerialParity.Space:
                    return PARITY.SPACE;
                default:
                    throw new Exception($"SerialParity {parity} is not supported for the FTDIDevice.");
            }
        }

        private static FLOW_CONTROL GetFlowControl(SerialHandshake handshake)
        {
            switch (handshake)
            {
                case SerialHandshake.None:
                    return FLOW_CONTROL.NONE;
                case SerialHandshake.RequestToSend:
                    return FLOW_CONTROL.RTS_CTS;
                case SerialHandshake.RequestToSendXOnXOff:
                    return FLOW_CONTROL.DTR_DSR; // todo: this is probably not correct...
                case SerialHandshake.XOnXOff:
                    return FLOW_CONTROL.XON_XOFF;
                default:
                    throw new Exception($"SerialHandshake {handshake} is not supported for the FTDIDevice.");
            }
        }

        public Task<uint> WriteAsync(byte[] bytesToWrite, uint nrBytesToWrite)
        {
            return ftDevice.WriteAsync(bytesToWrite, nrBytesToWrite).AsTask();
        }

        public uint GetQueueStatus()
        {
            return ftDevice.GetQueueStatus();
        }

        public async Task<IEnumerable<byte>> ReadAsync(byte[] buffer, uint bytesInQueue)
        {            
            var bytesRead = await ftDevice.ReadAsync(buffer, bytesInQueue).AsTask();
            return buffer.Take((int) bytesRead);
        }
    }
}