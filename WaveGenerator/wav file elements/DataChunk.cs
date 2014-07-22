﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class DataChunk : Chunk
    {        
        private Stream _file;
        private uint _byteCount = 0;
        private uint _dataOffset = 0;

        public override uint Size
        {
            get
            {
                uint size = _byteCount+(uint)(this._chunkID.Length + this._chunkDataSize.Length);
                return size;
            }           
        }        

        public DataChunk(Stream file, uint dataOffset)
            : base("data", 0)
        {
            this._file = file;
            _dataOffset = (uint)(dataOffset +
                                 this._chunkID.Length +
                                 this._chunkDataSize.Length);
        }

        public DataChunk(uint dataOffset)
            : base("data", 0)
        {
            this._dataOffset = (uint)(dataOffset +
                                 this._chunkID.Length +
                                 this._chunkDataSize.Length);
        }

        public void AddSamples(byte[] sample, long index)
        {            
            if (sample == null || _file == null)
                return;
            if (uint.MaxValue - 44 < _byteCount + sample.Length)
                throw new OverflowException("The file is too big");
            
            bool isThereATail = (_file.Length - _dataOffset - _byteCount) > 0;          
       
            _file.Position = _dataOffset + index*sample.Length;
           
            if (isThereATail && (_file.Position >= _dataOffset + _byteCount || _file.Position+sample.Length > _dataOffset + _byteCount))
            {
                
                byte[] tail = new byte[_file.Length - _dataOffset - _byteCount];
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

        public byte[] GetHeaderBytes()
        {
            this._chunkDataSize = BitConverter.GetBytes(_byteCount);
            byte[] result = Chunk.JoinByteArrays(base.GetChunkBytes());
            return result;
        }

        public override void LoadChunkBytes(FileStream file, int offSet)
        {
            base.LoadChunkBytes(file, offSet);
            _byteCount = this.ChunkDataSize;
            _file = file;
        }
    }
}