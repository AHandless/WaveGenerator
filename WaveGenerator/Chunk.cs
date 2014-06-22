using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    abstract class Chunk
    {
        protected byte[] _chunkID;
        protected byte[] _chunkDataSize;

        protected long byteCount;

        protected List<byte[]> arraysInOrder = new List<byte[]>();

        protected Chunk(string chunkID, uint chunkDataSize)
        {
            if (chunkID == null)
                throw new ArgumentNullException("chunkID", "Can't create a chunk withoud an ID");
            this._chunkID = Encoding.ASCII.GetBytes(chunkID);
            arraysInOrder.Add(this._chunkID);
            byteCount += this._chunkID.Length;


            this._chunkDataSize = BitConverter.GetBytes(chunkDataSize);
            arraysInOrder.Add(this._chunkDataSize);
            byteCount += this._chunkDataSize.Length;
        }

        public void ChangeSize(byte[] newSize)
        {
            _chunkDataSize = newSize;
        }

        abstract public byte[] GetChunkBytes();
        
    }
}
