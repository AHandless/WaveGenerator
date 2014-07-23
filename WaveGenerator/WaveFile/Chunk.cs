using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{    
    abstract class Chunk
    {
        protected byte[] _chunkID;
        protected byte[] _chunkDataSize;
        protected uint _chunkOffset;
        protected Stream _file;
        /// <summary>
        /// Chunk's size in bytes
        /// </summary>
        public abstract uint Size { get; }
        public string ChunkID
        {
            get
            {
                if (_chunkID != null)
                    return Encoding.ASCII.GetString(_chunkID);
                else
                    return null;
            }
        }
        public uint ChunkDataSize
        {
            get
            {
                if (_chunkDataSize != null)
                    return BitConverter.ToUInt32(_chunkDataSize, 0);
                else
                    return 0;
            }
        }

        protected Chunk(string chunkID, uint chunkDataSize, uint offset, Stream file)
        {
            if (chunkID == null)
                throw new ArgumentNullException("chunkID", "Can't create a chunk without an ID");
            this._chunkID = Encoding.ASCII.GetBytes(chunkID);
            this._chunkDataSize = BitConverter.GetBytes(chunkDataSize);
            this._chunkOffset = offset;
            this._file = file;
        }

        protected Chunk()
        {
            this._chunkID = new byte[4];
            this._chunkDataSize = new byte[4];       
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

        virtual public byte[] GetChunkBytes()
        {
            return JoinByteArrays(this._chunkID,
                                  this._chunkDataSize);
        }

        virtual public void LoadChunkBytes(Stream file, uint offSet)
        {
            file.Position = offSet;
            file.Read(this._chunkID, 0, 4);
            file.Read(this._chunkDataSize, 0, 4);
            this._chunkOffset = offSet;
        }
    }
}