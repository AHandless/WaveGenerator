using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    sealed class DataChunk : Chunk
    {
        private uint _byteCount = 0;
        private uint _dataOffset = 0;

        private byte[] _fileTail = null;

        private FormatChunk _format;

        public override uint Size
        {
            get
            {
                uint size = _byteCount + (uint)(this._chunkID.Length + this._chunkDataSize.Length);
                return size;
            }
        }
        public uint FileTailSize
        {
            get
            {
                if (_fileTail != null)
                    return (uint)_fileTail.Length;
                else
                    return 0;
            }
        }
        public byte PadByte
        {
            get
            {
                if (_byteCount % 2 == 0)
                    return 0;
                else
                    return 1;
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
        }

        public DataChunk(FormatChunk format)
            : base()
        {
            this._format = format;
            this._dataOffset = (uint)(this._chunkOffset + this._chunkID.Length + this._chunkDataSize.Length);
        }

        public void AddSampleToEnd(byte[] sample)
        {
            if (sample == null || _file == null)
                return;
            if (uint.MaxValue - 44 < _byteCount + sample.Length)
                throw new OverflowException("The file is too big");
            if (sample.Length != _format.ByteDepth)
                throw new FormatException(string.Format("The sample is in a wrong format. For this wave file a sample must be {0} bytes long, provided sample is {1} bytes long.", _format.ByteDepth, sample.Length));
            long newPosition = _dataOffset + _byteCount;
            if (newPosition != _file.Position)
                _file.Position = newPosition;
            for(int i = 0; i < _format.Channels; i++)                
                _file.Write(sample, 0, sample.Length);
            long bytesToAdd = _file.Position - (_dataOffset + _byteCount);
            if (bytesToAdd > 0)
                _byteCount += (uint)bytesToAdd;         
        }

        public byte[] GetSample(uint index, BitDepth bd)
        {
            byte[] result = null;
            result = new byte[(byte)bd / 8];
            _file.Position = _dataOffset + index * ((byte)bd / 8);
            _file.Read(result, 0, result.Length);
            return result;
        }

        protected override byte[] GetChunkBytes()
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
            uint fileTailSize = (uint)_file.Length - _dataOffset - _byteCount;
            if (fileTailSize > 0)
            {
                //is there the pad byte
                if (_byteCount % 2 > 0)
                {
                    fileTailSize--;
                    _fileTail = new byte[fileTailSize];
                    _file.Position = _dataOffset + this.ChunkDataSize+1;
                    _file.Read(_fileTail, 0, _fileTail.Length);
                }
                else
                {
                    _fileTail = new byte[fileTailSize];
                    _file.Position = _dataOffset + this.ChunkDataSize;
                    _file.Read(_fileTail, 0, _fileTail.Length);
                }
            }
        }

        public void Save()
        {
            _file.Position = _chunkOffset;
            byte[] chunkBytes = this.GetChunkBytes();
            _file.Write(chunkBytes, 0, chunkBytes.Length);

            _file.Position = _dataOffset + _byteCount;
            if (PadByte == 1)
                _file.WriteByte(0);
            //Tail
            if (_fileTail != null)
            {
                _file.Write(_fileTail, 0, _fileTail.Length);
            }
        }
    }
}