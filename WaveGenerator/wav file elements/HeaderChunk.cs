using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class HeaderChunk:Chunk
    {   
        byte[] _RIFFType = Encoding.ASCII.GetBytes("WAVE");

        public override uint Size
        {
            get
            {
                return (uint)(_RIFFType.Length+this._chunkDataSize.Length+this._chunkID.Length);
            }          
        }
        public string RIFFType
        {
            get
            {
                if (_RIFFType != null)
                    return Encoding.ASCII.GetString(_RIFFType);
                else
                    return null;
            }
        }

        public HeaderChunk(uint size)
            : base("RIFF", size)
        {
        }

        public HeaderChunk()
            : base("RIFF", 0)
        {
            _RIFFType = new byte[4];
        }

        public override byte[] GetChunkBytes()
        {
            byte[] result = Chunk.JoinByteArrays(base.GetChunkBytes(),
                                          this._RIFFType);            
            return result;
        }

        public override void LoadChunkBytes(System.IO.FileStream file, int offSet)
        {
            base.LoadChunkBytes(file, offSet);
            file.Position = offSet + this._chunkID.Length + this._chunkDataSize.Length;
            file.Read(this._RIFFType, 0, 4);
        }
    }
}