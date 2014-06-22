using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk : Chunk
    {
        List<byte[]> _samplesBytes = new List<byte[]>();   

        public DataChunk()
            : base("data", 0)
        {

        }        

        public void AddSamples(byte[] sample)
        {
            _samplesBytes.Add(sample);
            uint currentSize = BitConverter.ToUInt32(this._chunkDataSize, 0);
            currentSize += (uint)sample.Length;
            this._chunkDataSize = BitConverter.GetBytes(currentSize);
        }

        public override byte[] GetChunkBytes()
        {
            byte[] result = Chunk.JoinByteArrays(this.GetHeaderBytes(),
                                                 this.GetSampleBytes());           
            return result;
        }

        public byte[] GetSampleBytes()
        {
            byte[] data = Chunk.JoinByteArrays(_samplesBytes.ToArray());
            return data;
        }

        public byte[] GetHeaderBytes()
        {
            byte[] result = Chunk.JoinByteArrays(this._chunkID,
                                                 this._chunkDataSize);
            return result;
        }
    }
}
