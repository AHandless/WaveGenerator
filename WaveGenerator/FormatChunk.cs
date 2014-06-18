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
            this.arraysInOrder.Add(this._compressionCode);
            byteCount += this._compressionCode.Length;

            this._numberOfChannels = BitConverter.GetBytes(channels);
            byteCount += this._numberOfChannels.Length;
            this.arraysInOrder.Add(this._numberOfChannels);

            this._sampleRate = BitConverter.GetBytes(sampleRate);
            byteCount += this._sampleRate.Length;
            this.arraysInOrder.Add(this._sampleRate);

            _signigicantBitsPerSample = BitConverter.GetBytes(bitsPerSample);
            byteCount += this._signigicantBitsPerSample.Length;
            

            //_averageBytesPerSecond = sampleRate * (signigicantBitsPerSample / 8) * numberOfChannels;        
            //_blockAlign = (ushort)(numberOfChannels*2);  
            ushort BA = (ushort)(bitsPerSample / 8 * channels);
            _blockAlign = BitConverter.GetBytes(BA);
            byteCount += this._blockAlign.Length;
           
            _averageBytesPerSecond = BitConverter.GetBytes((uint)(sampleRate * BA));  
            byteCount += _averageBytesPerSecond.Length;
            
            this.arraysInOrder.Add(this._averageBytesPerSecond);
            this.arraysInOrder.Add(this._blockAlign);
            this.arraysInOrder.Add(this._signigicantBitsPerSample);

            //_extraFormatBytes = BitConverter.GetBytes((ushort)0);
            //this.arraysInOrder.Add(this._extraFormatBytes);
        }

        public override byte[] GetChunkBytes()
        {
            int pos = 0;
            byte[] result = new byte[byteCount];
            foreach (byte[] param in arraysInOrder)
            {
                param.CopyTo(result, pos);
                pos += param.Length;
            }
            return result;
        }
    }
}
