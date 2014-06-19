using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class SoundGenerator
    {
        private long _sampleCount = 0;
        private DataChunk data = new DataChunk();
        private HeaderChunk header;
        private FormatChunk format;

        private uint _sampleRate;
        private ushort _bitPerSample;
        private ushort _channels;

        public SoundGenerator(uint sampleRate, ushort bitPerSample, ushort channels)
        {
            this._sampleRate = sampleRate;
            this._bitPerSample = bitPerSample;
            this._channels = channels;
        }


        public void AddTone(double frequency, long duration)
        {
            long sampleCount = duration * _sampleRate / 1000 + 1;
            _sampleCount += sampleCount;
            double amplitude = Math.Pow(2, _bitPerSample) / 2 - 1;
            double t = Math.PI * 2 * frequency / _sampleRate;
            for (uint i = 0; i < sampleCount; i++)
            {
                double sin = amplitude * Math.Sin(t * i);
                byte[] sinBytes = ConvertNumber(sin, (byte)_bitPerSample);
                for (int channel = 0; channel < _channels; channel++)
                {
                    data.AddSamples(sinBytes);
                }
            }        
        }

        public byte[] ConvertNumber(double number, byte bit)
        {

            byte[] fullNumber = BitConverter.GetBytes(Convert.ToInt64(number));

            byte[] result = new byte[bit / 8]; 
            //It bit depth is 8
            if (bit == 8)
            {
                sbyte signed = Convert.ToSByte(number);
                byte unsigned = 0;                
                unsigned = (byte)(127+signed);                
                unsigned = (byte)(127+signed);
                result[0] = unsigned;
                return result;
            } 
            for (int i = 0; i < bit / 8; i++)
            {
                result[i] = fullNumber[i];
            }
            return result;
        }

        public void SaveTo(Stream stream)
        {
            uint fileSize = (uint)(4 + 24 + (8 + (_bitPerSample/8) * _channels * _sampleCount + 0));
            header = new HeaderChunk(fileSize);
            format = new FormatChunk(_sampleRate, _channels, _bitPerSample);

            byte[] headerbytes = header.GetChunkBytes();
            byte[] formatBytes = format.GetChunkBytes();
            byte[] dataBytes = data.GetChunkBytes();
            stream.Write(headerbytes, 0, headerbytes.Length);
            stream.Write(formatBytes, 0, formatBytes.Length);
            stream.Write(dataBytes, 0, dataBytes.Length);
            stream.Close();
        }
    }
}
