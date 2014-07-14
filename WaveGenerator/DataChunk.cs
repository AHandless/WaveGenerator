using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk : Chunk
    {        
        private Stream _file;
        private uint _byteCount = 0;
        private uint _dataOffset = 0;

        public override uint Size
        {
            get
            {
                uint size = _byteCount+(uint)(this._chunkID.Length + this._chunkDataSize.Length);
                return size;
            }           
        }

        public DataChunk(Stream file, int dataOffset)
            : base("data", 0)
        {
            this._file = file;
            _file.Position = dataOffset +
                             this._chunkID.Length +
                             this._chunkDataSize.Length;
            _dataOffset = (uint)_file.Position;
        }        

        public void AddSamples(byte[] sample)
        {
            if (sample == null)
                throw new ArgumentNullException("sample", "A sample can't be null");
            if (uint.MaxValue - 44 < _byteCount + sample.Length)
                throw new OverflowException("The file is too big");
            _file.Position = _dataOffset + _byteCount;
            _file.Write(sample, 0, sample.Length);
            _byteCount += (uint)sample.Length;                
            _file.Flush();
       
        }

        public override byte[] GetChunkBytes()
        {                  
            byte[] result = Chunk.JoinByteArrays(this.GetHeaderBytes());           
            return result;
        }       

        public byte[] GetHeaderBytes()
        {
            this._chunkDataSize = BitConverter.GetBytes(_byteCount);
            byte[] result = Chunk.JoinByteArrays(this._chunkID,
                                                 this._chunkDataSize);
            return result;
        }
    }
}
