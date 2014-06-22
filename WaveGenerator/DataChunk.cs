using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk : Chunk
    {
        List<byte[]> _samplesBytes = new List<byte[]>();

        public override uint Size
        {
            get
            {
                uint size = BitConverter.ToUInt32(this._chunkDataSize, 0)+
                                (uint)this._chunkID.Length+(uint)this._chunkDataSize.Length;
                return size;
            }           
        }

        public DataChunk()
            : base("data", 0)
        {

        }        

        public void AddSamples(byte[] sample)
        {
            if (sample == null)
                throw new ArgumentNullException("sample", "A sample can't be null");          
            _samplesBytes.Add(sample);
            uint currentSize = BitConverter.ToUInt32(this._chunkDataSize, 0);
            currentSize += (uint)sample.Length;
            this._chunkDataSize = BitConverter.GetBytes(currentSize);
        }

        public override byte[] GetChunkBytes()
        {
            byte[] sampleBytes = this.GetSampleBytes();
            byte[] padByte = null;
            if (sampleBytes.Length % 2 != 0)
                padByte = new byte[] {0};            
            byte[] result = Chunk.JoinByteArrays(this.GetHeaderBytes(),
                                                 sampleBytes,
                                                 padByte);           
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
