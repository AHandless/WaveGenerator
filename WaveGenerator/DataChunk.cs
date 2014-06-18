using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk
    {
        int chunkID = 0x64617461;//"DATA";
        int chunkSize = 0;
        short[] samples = null;
        FormatChunk format = new FormatChunk();
        WaveHeaderChunk header = null;

        public void AddTone(double frequency, int duration)
        {
            samples = new short[duration * (format.SampleRate/1000)];
           // The "angle" used in the function, adjusted for the number of channels and sample rate.
           // This value is like the period of the wave.
           double t = (Math.PI * 2 * frequency) / (format.SampleRate * format.Channes);
           double amplitude = Math.Pow(2, format.SignificantBitsPerSample) / 2 - 1;
            for (uint i = 0; i < samples.Length; i++)
            {
                for (int channel = 0; channel < format.Channes; channel++)
                {
                    samples[i] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                }
            }
            chunkSize = format.SignificantBitsPerSample / 8 * samples.Length;
            header = new WaveHeaderChunk(4 + 24 + (8 + 2 * format.Channes * samples.Length + 0));
        }

        public void SaveTo(string path)
        {
            FileStream file = new FileStream(path, FileMode.OpenOrCreate);
            //Заголовок
            bool f = BitConverter.IsLittleEndian;
            byte[] riff = BitConverter.GetBytes(header.chunkID);
            Array.Reverse(riff);
            byte[] cksize = BitConverter.GetBytes(header.chunckDataSize);
            byte[] waveID = BitConverter.GetBytes(header.RIFFType);
            Array.Reverse(waveID);
            file.Write(riff, 0, riff.Length);
            file.Write(cksize, 0, cksize.Length);
            file.Write(waveID, 0, waveID.Length);
            //format chunk
            byte[] fmt = BitConverter.GetBytes(format.chunkID);
            Array.Reverse(fmt);
            byte[] chzise = BitConverter.GetBytes(format.chunkDataSize);
            byte[] wformatTag = BitConverter.GetBytes(format.compressionCode);
            byte[] numberOfC = BitConverter.GetBytes(format.numberOfChannels);
            byte[] sampleRate = BitConverter.GetBytes(format.sampleRate);
            byte[] avebps = BitConverter.GetBytes(format.averageBytesPerSecond);
            byte[] nBlockalign = BitConverter.GetBytes(format.blockAlign);
            byte[] bitPS = BitConverter.GetBytes(format.signigicantBitsPerSample);
            byte[] DataId = BitConverter.GetBytes(chunkID);
            Array.Reverse(DataId);
            byte[] cksizeD = BitConverter.GetBytes(chunkSize);
            file.Write(fmt, 0, fmt.Length);
            file.Write(chzise, 0, chzise.Length);
            file.Write(wformatTag, 0, wformatTag.Length);
            file.Write(numberOfC, 0, numberOfC.Length);
            file.Write(sampleRate, 0, sampleRate.Length);
            file.Write(avebps, 0, avebps.Length);
            file.Write(nBlockalign, 0, nBlockalign.Length);
            file.Write(bitPS, 0, bitPS.Length);
            file.Write(DataId, 0, DataId.Length);
            file.Write(cksizeD, 0, cksizeD.Length);
            //samples
            foreach (short s in samples)
            {
                byte[] sample = BitConverter.GetBytes(s);
                file.Write(sample, 0, sample.Length);
            }

            file.Close();

        }

    }
}
