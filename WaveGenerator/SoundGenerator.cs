using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class SoundGenerator
    {
        private long _generatedSampleCount = 0;
        private DataChunk data = new DataChunk();
        private HeaderChunk header;
        private FormatChunk format;

        private uint _sampleRate;
        private ushort _bitPerSample;
        private ushort _channels;

        private long lastDataChunkPosition = 0;
        Stream _file;     

        public SoundGenerator(uint sampleRate, ushort bitPerSample, ushort channels, Stream file)
        {
            this._sampleRate = sampleRate;
            this._bitPerSample = bitPerSample;
            this._channels = channels;
            this._file = file;
        }

        public double phase = 0;
        public void AddTone(double frequency, long duration)
        {           
            long sampleCount = duration * _sampleRate / 1000;
            _generatedSampleCount += sampleCount;

            double amplitude = Math.Pow(2, _bitPerSample) / 2 - 1;
            double radPerSample = 2 * Math.PI / _sampleRate;
            if (_file != null)
            {
                DataChunk uncompleted = new DataChunk();
                for (uint i = 0; i < sampleCount; i++)
                {
                    double sin = amplitude * Math.Sin(phase);
                    phase += frequency * radPerSample;
                    byte[] sinBytes = ConvertNumber(sin, (byte)_bitPerSample);
                    for (int channel = 0; channel < _channels; channel++)
                    {                       
                       uncompleted.AddSamples(sinBytes);
                    }
                }
                //Сбрасывание промежуточных результатов
                _file.Position = 0;
                SaveHeadersToFile(_file);
                byte[] uncompleteDataBytes = uncompleted.GetSampleBytes();
                uncompleted.ChangeSize(BitConverter.GetBytes((uint)(_bitPerSample / 8 * (_generatedSampleCount*_channels))));
                byte[] uncompletedDataHeaderBytes = uncompleted.GetHeaderBytes();
                _file.Write(uncompletedDataHeaderBytes, 0, uncompletedDataHeaderBytes.Length);
                long headersEnd = _file.Position;
                _file.Position += lastDataChunkPosition;
                _file.Write(uncompleteDataBytes, 0, uncompleteDataBytes.Length);
                lastDataChunkPosition = _file.Position - headersEnd;
                //padByte
                if ((_generatedSampleCount*_channels*(_bitPerSample/8)) % 2 != 0)
                {
                    _file.Write(new byte[] { 0 }, 0, 1);
                }
                _file.Flush();
            }
            else
            {
                for (uint i = 0; i < sampleCount; i++)
                {
                    double sin = amplitude * Math.Sin(phase);
                    phase += frequency * radPerSample;
                    byte[] sinBytes = ConvertNumber(sin, (byte)_bitPerSample);
                    for (int channel = 0; channel < _channels; channel++)
                    {
                        data.AddSamples(sinBytes);                        
                    }
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
                unsigned = (byte)(127 + signed);
                unsigned = (byte)(127 + signed);
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
            header = new HeaderChunk(0);
            format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
            uint fileSize = (uint)(header.Size + format.Size + data.Size);
            //Check if there's the pad byte
            if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
            {
                fileSize += 1;
            }
            header.ChangeSize(BitConverter.GetBytes(fileSize));
            byte[] headerbytes = header.GetChunkBytes();
            byte[] formatBytes = format.GetChunkBytes();
            byte[] dataBytes = data.GetChunkBytes();
            stream.Write(headerbytes, 0, headerbytes.Length);
            stream.Write(formatBytes, 0, formatBytes.Length);
            stream.Write(dataBytes, 0, dataBytes.Length);
        }

        private void SaveHeadersToFile(Stream file)
        {
            file.Position = 0;           
            header = new HeaderChunk(0);
            format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
            uint fileSize = (uint)(header.Size + format.Size + 8+_channels*(_bitPerSample/8)*_generatedSampleCount);
            //Check if there's the pad byte
            if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
            {
                fileSize += 1;
            }
            header.ChangeSize(BitConverter.GetBytes(fileSize));
            byte[] headerbytes = header.GetChunkBytes();
            byte[] formatBytes = format.GetChunkBytes();
            file.Write(headerbytes, 0, headerbytes.Length);
            file.Write(formatBytes, 0, formatBytes.Length);
        }
    }
}
