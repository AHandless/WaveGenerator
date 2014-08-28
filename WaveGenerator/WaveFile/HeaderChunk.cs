using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    sealed class HeaderChunk:Chunk
    {   
        byte[] _RIFFType = Encoding.ASCII.GetBytes("WAVE");       
        FormatChunk _format;
        DataChunk _data;

        public override uint Size
        {
            get
            {
                return (uint)(_RIFFType.Length+_chunkDataSize.Length+_chunkID.Length);
            }          
        }
        public string RIFFType
        {
            get
            {
                if (_RIFFType != null)
                    return Encoding.ASCII.GetString(_RIFFType);
                else
                    return null;
            }
        }


        public HeaderChunk(Stream file, FormatChunk format, DataChunk data, uint offSet)
            : base("RIFF", 0, offSet, file)
        {           
            this._format = format;
            this._data = data;       
        }

        public HeaderChunk(FormatChunk format, DataChunk data)
            : base()
        {
            this._RIFFType = new byte[4];
            this._format = format;
            this._data = data;           
        }

        protected override byte[] GetChunkBytes()
        {
            byte[] result = Chunk.JoinByteArrays(base.GetChunkBytes(),
                                          this._RIFFType);            
            return result;
        }

        public void Save()
        {
            _file.Position = 0;
            uint fileSize = this.Size+_format.Size+_data.Size+_data.FileTailSize-8+_data.PadByte;
            this._chunkDataSize = BitConverter.GetBytes(fileSize);
            byte[] chunkBytes = this.GetChunkBytes();
            _file.Write(chunkBytes, 0, chunkBytes.Length);
        }

        public override void LoadChunkBytes(Stream file, uint offSet)
        {
            base.LoadChunkBytes(file, offSet);
            file.Position = offSet + this._chunkID.Length + this._chunkDataSize.Length;
            file.Read(this._RIFFType, 0, 4);
            this._file = file;         
        }
    }
}