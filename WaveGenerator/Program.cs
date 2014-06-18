using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //На вход
            uint sampleRate = 44100;
            ushort bitPerSample = 16;
            ushort channels = 1;

            FormatChunk format = new FormatChunk(sampleRate, channels, bitPerSample);
            DataChunk data = new DataChunk();

            int duration = 10000;
            double frequency = 1500;
            long sampleCount = duration * (sampleRate / 1000);
            short[] samples = new short[sampleCount];            
           
            // The "angle" used in the function, adjusted for the number of channels and sample rate.
            // This value is like the period of the wave.
            double t = (Math.PI * 2 * frequency) / (sampleRate * channels);
            double amplitude = Math.Pow(2, bitPerSample) / 2 - 1;
            for (uint i = 0; i < samples.Length; i++)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    samples[i] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                }
            }
            data.AddSamples(samples);
            
            uint fileSize = (uint)(4 + 24 + (8 + 2 * channels * samples.Length + 0));
            HeaderChunk header = new HeaderChunk(fileSize);

            FileStream file = new FileStream("test.wav", FileMode.OpenOrCreate);
            byte[] headerBytes = header.GetChunkBytes();
            byte[] formatBytes = format.GetChunkBytes();
            byte[] dataBytes = data.GetChunkBytes();
            file.Write(headerBytes, 0, headerBytes.Length);
            file.Write(formatBytes, 0, formatBytes.Length);
            file.Write(dataBytes, 0, dataBytes.Length);
            file.Close();


        }
    }
}
