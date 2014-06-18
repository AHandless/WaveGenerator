using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk:Chunk
    {
        List<short[]> _samples = new List<short[]>();
        long _allSamplesCount = 0;     

        public DataChunk()
            : base("data", 0)
        {

        }

        public void AddSamples(short[] samples)
        {
            this._samples.Add(samples);
            _allSamplesCount += samples.Length;
        }

        public override byte[] GetChunkBytes()
        {          
            byte[] sampleBytes = new byte[_allSamplesCount * 2];           
            int pos = 0;
            foreach (short[] shortSamples in _samples)
            {
                foreach (short s in shortSamples)
                {
                    byte[] byteSample = BitConverter.GetBytes(s);
                    byteSample.CopyTo(sampleBytes, pos);
                    pos += byteSample.Length;
                }               
            }          
            byteCount += sampleBytes.Length;
            //Get size of data chunk
            this._chunkDataSize = BitConverter.GetBytes((uint)sampleBytes.Length);
            pos = 0;
            byte[] result = new byte[byteCount];
            
            this._chunkID.CopyTo(result, pos);
            pos += this._chunkID.Length;
            this._chunkDataSize.CopyTo(result, pos);
            pos += this._chunkDataSize.Length;
            sampleBytes.CopyTo(result, pos);
            
            return result;
        }
    }
}
