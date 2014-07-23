using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk : Chunk
    {   
        private uint _byteCount = 0;
        private uint _dataOffset = 0;
        private uint _fileTailSize = 0;

        private FormatChunk _format;

        public override uint Size
        {
            get
            {
                uint size = _byteCount+(uint)(this._chunkID.Length + this._chunkDataSize.Length);
                return size;
            }           
        }
        public uint FileTailSize
        {
            get
            {
                if (_fileTailSize > 0)
                    return _fileTailSize;
                else
                    return 0;
            }
        }

        public DataChunk(Stream file, uint dataOffset, FormatChunk format)
            : base("data", 0, dataOffset, file)
        {
            this._format = format;          
            this._chunkOffset = dataOffset;
            _dataOffset = (uint)(this._chunkOffset +
                                 this._chunkID.Length +
                                 this._chunkDataSize.Length);
            this._fileTailSize = (uint)(_file.Length - _dataOffset - _byteCount);
        }

        public DataChunk(FormatChunk format)
            : base()
        {
            this._format = format;        
            this._dataOffset = (uint)(this._chunkOffset + this._chunkID.Length +this._chunkDataSize.Length);            
        }

        public void AddSamples(byte[] sample, long index, byte channel)
        {            
            if (sample == null || _file == null)
                return;
            if (uint.MaxValue - 44 < _byteCount + sample.Length)
                throw new OverflowException("The file is too big");            
           
            long tailLength = _file.Length - _dataOffset - _byteCount;
  
       
            _file.Position = _dataOffset + index*sample.Length*_format.Channels+sample.Length*channel;
           
            if (tailLength>0 && (_file.Position >= _dataOffset + _byteCount || _file.Position+sample.Length > _dataOffset + _byteCount))
            {                
                byte[] tail = new byte[tailLength];
                _file.Position = _file.Length - tail.Length;
                _file.Read(tail, 0, tail.Length);
                _file.Position = _dataOffset + index * sample.Length;
                _file.Write(sample, 0, sample.Length);
                long bytesToAdd = _file.Position - (_dataOffset + _byteCount);
                if (bytesToAdd > 0)
                    _byteCount += (uint)bytesToAdd;
                _file.Write(tail, 0, tail.Length);               
            }
            else
            {                        
                _file.Write(sample, 0, sample.Length);
                long bytesToAdd = _file.Position - (_dataOffset + _byteCount);
                if (bytesToAdd > 0)
                    _byteCount += (uint)bytesToAdd;
            }
            _file.Flush();       
        }

        public byte[] GetSample(uint index, BitDepth bd)
        {
            byte[] result = null;            
            result = new byte[(byte)bd/8];
            _file.Position = _dataOffset + index*((byte)bd/8);
            _file.Read(result, 0, result.Length);             
            return result;
        }

        public override byte[] GetChunkBytes()
        {                  
            byte[] result = Chunk.JoinByteArrays(this.GetHeaderBytes());           
            return result;
        }       
     
        private byte[] GetHeaderBytes()
        {
            this._chunkDataSize = BitConverter.GetBytes(_byteCount);
            byte[] result = Chunk.JoinByteArrays(base.GetChunkBytes());
            return result;
        }

        public override void LoadChunkBytes(Stream file, uint offSet)
        {
            base.LoadChunkBytes(file, offSet);
            _dataOffset = (uint)(this._chunkOffset + this._chunkID.Length + this._chunkDataSize.Length);
            _byteCount = this.ChunkDataSize;
            _file = file;
            this._fileTailSize = (uint)(_file.Length - _dataOffset - _byteCount);
        }

        public void Save()
        {
            _file.Position = _chunkOffset;
            byte[] chunkBytes = this.GetChunkBytes();
            _file.Write(chunkBytes, 0, chunkBytes.Length);
        }
    }
}