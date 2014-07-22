using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class SoundGenerator
    {
        private uint _generatedSampleCount = 0;
        private DataChunk _data;
        private HeaderChunk _header;
        private FormatChunk _format;

        private uint _sampleRate;
        private ushort _bitPerSample;
        private ushort _channels;
       
        Stream _file;

        public SoundGenerator(uint sampleRate, BitDepth bitDepth, ushort channels, Stream file)
        {
            if (Enum.IsDefined(typeof(BitDepth), bitDepth) == false)
                throw new ArgumentException("Unsupported bit depth", "bitDepth");
            this._sampleRate = sampleRate;
            this._bitPerSample = (ushort)bitDepth;
            this._channels = channels;
            
            this._file = file;
            
            this._header = new HeaderChunk(0);
            this._format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
            this._data = new DataChunk(this._file, _header.Size+_format.Size);        
        }

        public SoundGenerator()
        {
            _header = new HeaderChunk();
            _format = new FormatChunk();
            _data = new DataChunk(_header.Size + _format.Size);
        }

        public double AddSimpleTone(double frequency, double duration, double startPhase, double amplitude, bool fade)
        {
            if (duration == 0)
                return 0;
            double lastPhase = 0;
            uint sampleCount = (uint)Math.Floor(duration * _sampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double fileAmplitude = Math.Pow(2, _bitPerSample - 1) - 1;
            double radPerSample = 2 * Math.PI / _sampleRate;

            double[] wave = GenerateSineWave(frequency, sampleCount, radPerSample, startPhase, out lastPhase);
            //Fading            
            uint end = (uint)(sampleCount * 0.1d);
            if (fade)
            {
                Fade(ref wave, 0, amplitude, 0, end);
                Fade(ref wave, amplitude, 0, (uint)(wave.Length - end), end);               
            }
            else
                end = 0;
            for (uint i = 0; i < wave.Length; i++)
            {               
                double sin = fileAmplitude * wave[i];
                if (i >= end && i < wave.Length - end)
                    sin *= amplitude;
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_bitPerSample);
                for (int channel = 0; channel < _channels; channel++)
                {
                    _data.AddSamples(sinBytes);
                }
            }
            if (fade)
                lastPhase = 0;
            return lastPhase;
        }          

        private void Fade(ref double[] wave, double startAmplitude, double endAmplitude, uint start, uint length)
        {
            if (start >= wave.Length ||  length > wave.Length-start)
                throw new ArgumentException("Start or end index was outside of the wave array for some reason");
            if (startAmplitude < 0 || endAmplitude < 0)
                throw new ArgumentException("The amplitude can't be negative");
            double minusAmpMax = 0;
         
            if (startAmplitude > endAmplitude)
            {
                minusAmpMax = startAmplitude - endAmplitude;
                for (int i = 0; i < length; i++)
                    wave[start + i] = wave[start + i] * (startAmplitude - (minusAmpMax / length * i));
            }
            else
            {
                minusAmpMax = endAmplitude - startAmplitude;
                for (int i = 0; i < length; i++)
                    wave[start + i] = wave[start + i] * (startAmplitude + (minusAmpMax / length * i));
            }
        }

        public double[] AddComplexTone(double duration, double[] startPhases, double amplitude, bool fade, params double[] frequencies)
        {
            if (duration == 0 || 
                double.IsInfinity(duration)||
                double.IsNaN(duration)||
                amplitude < 0 || 
                amplitude > 1 ||
                frequencies == null)
                return startPhases;
            uint sampleCount = (uint)Math.Floor(duration * _sampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double[] lastPhases = new double[frequencies.Length];
            double fileAmplitude = Math.Pow(2, _bitPerSample - 1) - 1;
            double radPerSample = 2 * Math.PI / _sampleRate;
            double[] complexWave = new double[sampleCount];
            for (int frequencyN = 0; frequencyN < frequencies.Length; frequencyN++)
            {
                double[] wave = GenerateSineWave(frequencies[frequencyN], sampleCount, radPerSample, startPhases[frequencyN], out lastPhases[frequencyN]);
                for (int i = 0; i < wave.Length; i++)
                    complexWave[i] += wave[i] / frequencies.Length;
            }
            //Fading            
            uint end = (uint)(sampleCount * 0.1d);
            if (fade)
            {
                Fade(ref complexWave, 0, amplitude, 0, end);
                Fade(ref complexWave, amplitude, 0, (uint)(complexWave.Length - end), end);              
            }
            else
                end = 0;
            for (int i = 0; i < complexWave.Length; i++)
            {
                double sin = complexWave[i];
                if (i >= end && i < complexWave.Length - end)
                    sin *= amplitude;
                sin = sin * fileAmplitude;
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_bitPerSample);
                for (int channel = 0; channel < _channels; channel++)
                {
                    _data.AddSamples(sinBytes);
                }
            }
            if (fade)
                for (int i = 0; i < lastPhases.Length; i++)
                    lastPhases[i] = 0;           
            return lastPhases;
        }

        private double[] GenerateSineWave(double frequency, uint length, double xInc, double startPhase, out double endPhase)
        {
            double[] wave = new double[length];           
            for (int x = 0; x < length; x++)
                wave[x] = Math.Sin(frequency * xInc * x + startPhase);
            endPhase = GetPhase(length * xInc * frequency + startPhase);
            return wave;
        }

        private double[] GenerateSquareWave(double frequency, int length, double xInc, double startPhase, out double endPhase)
        {
            double[] squareSineWave = new double[length];
            for (int i = 0; i < squareSineWave.Length; i++)
            {
                squareSineWave[i] = Math.Sign(Math.Sin(frequency * xInc * i + startPhase));
            }
            endPhase = GetPhase(length * xInc * frequency + startPhase);
            return squareSineWave;
        }

        private double[] GenerateSawtoothWave(double frequency, int length, double xInc, double startPhase, out double endPhase)
        {
            double[] wave = new double[length];
            double period = Math.PI * 2;
            for (int x = 0; x < length; x++)
            {
                double t = frequency * x * xInc + startPhase;
                wave[x] = 2 * (t / period - Math.Floor(1 / 2 + t / period));
            }
            endPhase = GetPhase(length * xInc * frequency + startPhase);
            return wave;
        }

        private double[] GenerateTriangleWave(double frequency, int length, double xInc, double startPhase, out double endPhase)
        {
            double[] wave = new double[length];
            double period = Math.PI / 2;
            for (int x = 0; x < length; x++)
            {
                double t = frequency * x * xInc + startPhase;
                wave[x] = 2 / Math.PI * Math.Asin(Math.Sin(Math.PI / 2 / period * t));
            }
            endPhase = GetPhase(length * xInc * frequency + startPhase);
            return wave;
        }

        private double GetPhase(double x)
        {
            double result = 0;
            double sint = Math.Sin(x);
            double cost = Math.Cos(x);
            if (cost > 0 || sint == -1)
                result = Math.Asin(sint);
            else
                result = -Math.Asin(sint) + Math.PI;
            return result;
        }        

        private byte[] ConvertNumber(long number, byte bit)
        {
            byte[] fullNumber = BitConverter.GetBytes(number);

            byte[] result = new byte[bit / 8];
            //It bit depth is 8
            if (bit == 8)
            {
                sbyte signed = Convert.ToSByte(number);
                byte unsigned = 0;
                unsigned = (byte)(128 + signed);
                result[0] = unsigned;
                return result;
            }
            for (int i = 0; i < bit / 8; i++)
            {
                result[i] = fullNumber[i];
            }
            return result;
        } 

        public void Save()
        {            
            //Check if there's the pad byte
            bool padByte = (_data.Size-8) % 2 != 0;
            if (padByte)
                _file.WriteByte(0);
            _file.Position = 0;            
            uint fileSize = (uint)((_header.Size-8) + _format.Size + _data.Size);         
            if (padByte)          
                fileSize += 1;      
            _header.ChangeSize(BitConverter.GetBytes(fileSize));
            byte[] headerbytes = _header.GetChunkBytes();
            byte[] formatBytes = _format.GetChunkBytes();
            byte[] dataBytes = _data.GetHeaderBytes();
           _file.Write(headerbytes, 0, headerbytes.Length);
           _file.Write(formatBytes, 0, formatBytes.Length);
           _file.Write(dataBytes, 0, dataBytes.Length);          
        }

        public void Load(FileStream file)
        {
            _header.LoadChunkBytes(file, 0);
            _format.LoadChunkBytes(file, (int)_header.Size);
            _data.LoadChunkBytes(file, (int)(_header.Size+_format.Size));
            _generatedSampleCount = (uint)(_data.ChunkDataSize / ((byte)_format.BitDepth / 8)/_format.Channels);
            _sampleRate = _format.SampleRate;
            _bitPerSample = (byte)_format.BitDepth;
            _channels = _format.Channels;
            _file = file;
        }
    }

    public enum BitDepth : byte
    {
        Bit8 = 8,
        Bit16 = 16,
        Bit24 = 24,
        Bit32 = 32
    }
}