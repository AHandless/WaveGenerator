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
            this._chunkID.CopyTo(result, 0);
            this._chunkDataSize.CopyTo(result, this._chunkID.Length);
            this.RIFFType.CopyTo(result, this._chunkID.Length + this._chunkDataSize.Length);
            return result;
        }
       
    }
}
