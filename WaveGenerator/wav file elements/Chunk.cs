using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{    
    abstract class Chunk
    {
        protected byte[] _chunkID;
        protected byte[] _chunkDataSize;
        /// <summary>
        /// Chunk's size in bytes
        /// </summary>
        public abstract uint Size { get; }

        protected Chunk(string chunkID, uint chunkDataSize)
        {
            if (chunkID == null)
                throw new ArgumentNullException("chunkID", "Can't create a chunk without an ID");
            this._chunkID = Encoding.ASCII.GetBytes(chunkID);
            this._chunkDataSize = BitConverter.GetBytes(chunkDataSize);         
        }

        public void ChangeSize(byte[] newSize)
        {
            if (newSize == null)
                _chunkDataSize = BitConverter.GetBytes((uint)0);
            _chunkDataSize = newSize;
        }

        protected static byte[] JoinByteArrays(params Array[] arrays)
        {          
            if (arrays == null)
                return null;
            long size = 0;
            foreach (var array in arrays)
            {
                if (arrays != null && array is byte[])
                {
                    size += array.Length;
                }
            }
            byte[] result = new byte[size];
            int position = 0;
            foreach (var array in arrays)
            {
                if (arrays != null && array is byte[])
                {
                    array.CopyTo(result, position);
                    position += array.Length;
                }
            }
            return result;
        }

        abstract public byte[] GetChunkBytes();        
    }
}