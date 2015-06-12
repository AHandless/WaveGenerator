using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class WaveFile
    {
        HeaderChunk _header;
        FormatChunk _format;
        DataChunk _data;
        Stream _file;

        public uint SampleRate
        {
            get
            {
                return _format.SampleRate;
            }
        }
        public ushort Channels
        {
            get
            {
                return _format.Channels;
            }
        }
        public BitDepth BitDepth
        {
            get
            {
                return _format.BitDepth;
            }
        }
        public Stream File
        {
            get
            {
                return _file;
            }
        }
        public uint SampleCount
        {
            get
            {
                return _data.Size / _format.Channels / _format.ByteDepth;
            }
        }

        public WaveFile()
        {
            _format = new FormatChunk();
            _data = new DataChunk(_format);
            _header = new HeaderChunk(_format, _data);
        }

        public WaveFile(uint sampleRate, BitDepth bitDepth, ushort channels, Stream file)
        {
            _format = new FormatChunk(sampleRate, channels, (ushort)bitDepth, file, 12);
            _data = new DataChunk(file, 12 + _format.Size, _format);
            _header = new HeaderChunk(file, _format, _data, 0);
            _file = file;
        }

        public void LoadFromFile(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            FileStream file = new FileStream(filePath, FileMode.Open);
            _header.LoadChunkBytes(file, 0);
            _format.LoadChunkBytes(file, _header.Size);
            _data.LoadChunkBytes(file, _header.Size + _format.Size);
            _file = file;
        }

        public void AddSampleToEnd(byte[] sample)
        {
            _data.AddSampleToEnd(sample);
        }

        public void AddSample(byte[] sample, int sampleNumber, int channel)
        {
            _data.AddSample(sample, sampleNumber, channel);
        }

        public void Save()
        {
            _header.Save();
            _format.Save();
            _data.Save();
        }
    }
}
