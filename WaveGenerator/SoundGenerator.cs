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
            short[] samples = new short[sampleCount*_channels];
            // The "angle" used in the function, adjusted for the number of channels and sample rate.
            // This value is like the period of the wave.
            double t = Math.PI * 2 * frequency / _sampleRate;          
            for (uint i = 0; i < samples.Length; i++)
            {
                for (int channel = 0; channel < _channels; channel++)
                {
                    samples[i] = Convert.ToInt16(short.MaxValue * Math.Sin(t * i));
                }
            }
          

            data.AddSamples(samples);
            _sampleCount += samples.Length;
        }

        public void SaveTo(Stream stream)
        {
            uint fileSize = (uint)(4 + 24 + (8 + 2 * _channels * _sampleCount*3 + 0));
            header = new HeaderChunk(fileSize);
            format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
          
            byte[] headerbytes = header.GetChunkBytes();
            byte[] formatBytes = format.GetChunkBytes();
            byte[] dataBytes = data.GetChunkBytes();
            stream.Write(headerbytes, 0, headerbytes.Length);
            stream.Write(formatBytes, 0, formatBytes.Length);
            stream.Write(dataBytes, 0, dataBytes.Length);
            //stream.Close();
        }
    }
}
