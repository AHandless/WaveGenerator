using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class FormatChunk
    {
        public int chunkID = 0x666D7420; //"fmt "
        public int chunkDataSize = 16;
        public ushort compressionCode = 1;       
        public ushort numberOfChannels = 1;       
        public int sampleRate = 44100;    
        public int averageBytesPerSecond = 1;
        public ushort blockAlign = 1;        
        public ushort signigicantBitsPerSample = 16;       
        public ushort extraFormatBytes = 0;


        public ushort Channes
        {
            get { return numberOfChannels; }
        }
        public ushort SignificantBitsPerSample
        {
            get { return signigicantBitsPerSample; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
        }
        public FormatChunk()
        {
            signigicantBitsPerSample = 8 * 2;
            averageBytesPerSecond = sampleRate * (signigicantBitsPerSample / 8) * numberOfChannels;        
            blockAlign = (ushort)(numberOfChannels*2);          
        }
    }
}
