using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class WaveHeaderChunk
    {
        public int chunkID = 0x52494646; //"RIFF"
        public int chunckDataSize = 0;
        public int RIFFType = 0x57415645; //"WAVE"

        public WaveHeaderChunk(int size)
        {
            chunckDataSize = size;
        }
    }
}
