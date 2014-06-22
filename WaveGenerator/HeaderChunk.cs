using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class HeaderChunk:Chunk
    {   
        byte[] RIFFType = Encoding.ASCII.GetBytes("WAVE");

        public HeaderChunk(uint size)
            : base("RIFF", size)
        {
        }

        public override byte[] GetChunkBytes()
        {
            byte[] result = new byte[this._chunkID.Length + this._chunkDataSize.Length+this.RIFFType.Length];
            result = Chunk.JoinByteArrays(this._chunkID,
                                          this._chunkDataSize,
                                          this.RIFFType);            
            return result;
        }       
    }
}
