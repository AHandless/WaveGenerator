using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    sealed class FormatChunk:Chunk
    {
        byte[] _compressionCode;
        byte[] _numberOfChannels;
        byte[] _sampleRate;
        byte[] _averageBytesPerSecond;
        byte[] _blockAlign;
        byte[] _signigicantBitsPerSample;
        byte[] _extraFormatBytes;        

        public override uint Size
        {
            get
            {
                uint size = (uint)(_chunkID.Length+
                                   _chunkDataSize.Length+
                                   _compressionCode.Length+
                                   _numberOfChannels.Length+
                                   _sampleRate.Length+
                                   _averageBytesPerSecond.Length+
                                   _blockAlign.Length+
                                   _signigicantBitsPerSample.Length);
                if (_extraFormatBytes != null)
                    size += (uint)_extraFormatBytes.Length;
                return size;
            }           
        }
        public ushort CompressionCode
        {
            get
            {
                if (_compressionCode != null)
                    return BitConverter.ToUInt16(_compressionCode, 0);
                else
                    return 0;
            }
        }
        public ushort Channels
        {
            get
            {
                if (_numberOfChannels != null)
                    return BitConverter.ToUInt16(_numberOfChannels, 0);
                else
                    return 0;
            }
        }
        public uint SampleRate
        {
            get
            {
                if (_sampleRate != null)
                    return BitConverter.ToUInt32(_sampleRate, 0);
                else
                    return 0;
            }
        }
        public uint AverageBytesPerSecond
        {
            get
            {
                if (_averageBytesPerSecond != null)
                    return BitConverter.ToUInt32(_averageBytesPerSecond, 0);
                else
                    return 0;
            }
        }
        public ushort BlockAlign
        {
            get
            {
                if (_blockAlign != null)
                    return BitConverter.ToUInt16(_blockAlign, 0);
                else
                    return 0;
            }
        }
        public BitDepth BitDepth
        {
            get
            {
                if (_signigicantBitsPerSample != null)
                    return (BitDepth)BitConverter.ToUInt16(_signigicantBitsPerSample, 0);
                else
                    return 0;
            }
        }

        public FormatChunk(uint sampleRate, ushort channels, ushort bitsPerSample, Stream file, uint offset):base("fmt ", 16, offset, file)
        {
            this._compressionCode = BitConverter.GetBytes((ushort)1);          

            this._numberOfChannels = BitConverter.GetBytes(channels);      

            this._sampleRate = BitConverter.GetBytes(sampleRate);         

            this._signigicantBitsPerSample = BitConverter.GetBytes(bitsPerSample);
          
            ushort BA = (ushort)(bitsPerSample / 8 * channels);
            _blockAlign = BitConverter.GetBytes(BA);      
           
            _averageBytesPerSecond = BitConverter.GetBytes((uint)(sampleRate * BA));                 
        }

        public FormatChunk()
            : base()
        {
            _compressionCode = new byte[2];
            _numberOfChannels = new byte[2];
            _sampleRate = new byte[4];
            _averageBytesPerSecond = new byte[4];
            _blockAlign = new byte[2];
            _signigicantBitsPerSample = new byte[2];          
        }

        protected override byte[] GetChunkBytes()
        {            
            byte[] result = result = Chunk.JoinByteArrays(base.GetChunkBytes(),
                                                          this._compressionCode,
                                                          this._numberOfChannels,
                                                          this._sampleRate,
                                                          this._averageBytesPerSecond,
                                                          this._blockAlign,
                                                          this._signigicantBitsPerSample,
                                                          this._extraFormatBytes);
            return result;
        }

        public void Save()
        {
            byte[] chunkBytes = this.GetChunkBytes();
            _file.Position = _chunkOffset;
            _file.Write(chunkBytes, 0, chunkBytes.Length);
        }

        public override void LoadChunkBytes(Stream file, uint offSet)
        {
            base.LoadChunkBytes(file, offSet);
            file.Position = offSet + this._chunkID.Length + this._chunkDataSize.Length;
            
            file.Read(_compressionCode, 0, 2);          
            file.Read(_numberOfChannels, 0, 2);
            file.Read(_sampleRate, 0, 4);
            file.Read(_averageBytesPerSecond, 0, 4);
            file.Read(_blockAlign, 0, 2);
            file.Read(_signigicantBitsPerSample, 0, 2);
            this._file = file;
        }
    }
}