using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class HeaderChunk:Chunk
    {   
        byte[] RIFFType = Encoding.ASCII.GetBytes("WAVE");

        public override uint Size
        {
            get
            {
                return (uint)(RIFFType.Length+this._chunkDataSize.Length+this._chunkID.Length);
            }          
        }

        public HeaderChunk(uint size)
            : base("RIFF", size)
        {
        }

        public override byte[] GetChunkBytes()
        {
            byte[] result = Chunk.JoinByteArrays(this._chunkID,
                                          this._chunkDataSize,
                                          this.RIFFType);            
            return result;
        }       
    }
}
