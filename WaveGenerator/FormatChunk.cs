using System;
using System.Collections.Generic;
using System.Text;

namespace WaveGenerator
{
    class FormatChunk:Chunk
    {       
        //ushort _compressionCode = 1;       
        //ushort _numberOfChannels;       
        //uint _sampleRate;    
        //uint _averageBytesPerSecond;
        //ushort _blockAlign;        
        //ushort _signigicantBitsPerSample;       
        //ushort _extraFormatBytes = 0;

        byte[] _compressionCode;
        byte[] _numberOfChannels;
        byte[] _sampleRate;
        byte[] _averageBytesPerSecond;
        byte[] _blockAlign;
        byte[] _signigicantBitsPerSample;
        byte[] _extraFormatBytes;        
       

        public FormatChunk(uint sampleRate, ushort channels, ushort bitsPerSample):base("fmt ", 16)
        {
            this._compressionCode = BitConverter.GetBytes((ushort)1);          

            this._numberOfChannels = BitConverter.GetBytes(channels);      

            this._sampleRate = BitConverter.GetBytes(sampleRate);         

            _signigicantBitsPerSample = BitConverter.GetBytes(bitsPerSample);
          
            ushort BA = (ushort)(bitsPerSample / 8 * channels);
            _blockAlign = BitConverter.GetBytes(BA);      
           
            _averageBytesPerSecond = BitConverter.GetBytes((uint)(sampleRate * BA));       
        }

        public override byte[] GetChunkBytes()
        {            
            byte[] result = result = Chunk.JoinByteArrays(this._chunkID,
                                                          this._chunkDataSize,
                                                          this._compressionCode,
                                                          this._numberOfChannels,
                                                          this._sampleRate,
                                                          this._averageBytesPerSecond,
                                                          this._blockAlign,
                                                          this._signigicantBitsPerSample,
                                                          this._extraFormatBytes);
            return result;
        }
    }
}
