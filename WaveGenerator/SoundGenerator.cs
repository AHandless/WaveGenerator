using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class SoundGenerator
    {
        private long _generatedSampleCount = 0;
        private DataChunk _data = new DataChunk();
        private HeaderChunk _header;
        private FormatChunk _format;

        private uint _sampleRate;
        private ushort _bitPerSample;
        private ushort _channels;

        //private long lastDataChunkPosition = 0;
        Stream _file;

        public SoundGenerator(uint sampleRate, ushort bitPerSample, ushort channels, Stream file)
        {
            this._sampleRate = sampleRate;
            this._bitPerSample = bitPerSample;
            this._channels = channels;
            this._file = file;
        }

        //double lastSin = 0d;
        //bool directionUp = true;

        public double AddSimpleTone(double frequency, double duration, double startPhase, double startJointAmp, double endJointAmp, double aM)
        {
            double lastPhase = 0;
            long sampleCount = (long)Math.Floor(duration * _sampleRate / 1000d);
            this._generatedSampleCount += sampleCount;
            double amplitudeMax = Math.Pow(2, _bitPerSample - 1) - 1;
            double radPerSample = 2 * Math.PI / _sampleRate;

            double[] wave = GenerateSineWave(frequency, (int)sampleCount, radPerSample, startPhase, out lastPhase);
            //Amplitude
            int end = 0;           
                Fade(ref wave, aM, endJointAmp, wave.Length - (int)(sampleCount * 0.1d) - 1, wave.Length - 1);
                Fade(ref wave, startJointAmp, aM, 0, (int)(sampleCount * 0.1d));
                end = (int)(sampleCount * 0.1d);
           
            for (int i = end+1; i < wave.Length-end; i++)
                wave[i] = wave[i] * aM;         
            for (uint i = 0; i < wave.Length; i++)
            {
                double sin = amplitudeMax * wave[i];
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_bitPerSample);
                for (int channel = 0; channel < _channels; channel++)
                {
                    _data.AddSamples(sinBytes);
                }
            }
            if (endJointAmp == 0)
                lastPhase = 0;
            return lastPhase;
        }

        private void Fade(ref double[] wave, double startAmplitude, double endAmplitude, int start, int end)
        {
            if (end < start)
                throw new ArgumentException("The end point can't be less than the start point");
            if (startAmplitude == endAmplitude || startAmplitude < 0 || endAmplitude < 0)
                return;
            double minusAmpMax;
            int fadeLength = end - start;
            if (startAmplitude > endAmplitude)
            {
                minusAmpMax = startAmplitude - endAmplitude;
                for (int i = 0; i <= fadeLength; i++)
                    wave[start + i] = wave[start + i] * (startAmplitude - (minusAmpMax / fadeLength * i));
            }
            else
            {
                minusAmpMax = endAmplitude - startAmplitude;
                for (int i = 0; i <= fadeLength; i++)
                    wave[start + i] = wave[start + i] * (startAmplitude + (minusAmpMax / fadeLength * i));
            }
        }         

        public double[] AddComplexTone(double duration, double[] startPhases, double startJointAmp, double endJointAmp, double aM, params double[] frequencies)
        {
            long sampleCount = (long)Math.Floor(duration * _sampleRate / 1000d);
            this._generatedSampleCount += sampleCount;
            double[] lastPhases = new double[frequencies.Length];
            double amplitude = Math.Pow(2, _bitPerSample - 1) - 1;
            double radPerSample = 2 * Math.PI / _sampleRate;
            double[] complexWave = new double[sampleCount];
            for (int f = 0; f < frequencies.Length; f++)
            {
                double[] wave = GenerateSineWave(frequencies[f], (int)sampleCount, radPerSample, startPhases[f], out lastPhases[f]);
                for (int i = 0; i < wave.Length; i++)              
                    complexWave[i] += wave[i]/frequencies.Length;
               
            }
            int end = 0;
            Fade(ref complexWave, aM, endJointAmp, complexWave.Length - (int)(sampleCount * 0.1d) - 1, complexWave.Length - 1);
            Fade(ref complexWave, startJointAmp, aM, 0, (int)(sampleCount * 0.1d));
            end = (int)(sampleCount * 0.1d);

            for (int i = end + 1; i < complexWave.Length - end; i++)
                complexWave[i] = complexWave[i] * aM;
            for (int i = 0; i < complexWave.Length; i++)
            {
                double sin = complexWave[i];
                sin = sin * amplitude;
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_bitPerSample);
                for (int channel = 0; channel < _channels; channel++)
                {
                    _data.AddSamples(sinBytes);
                }
            }
            if (endJointAmp == 0)
                for (int i = 0; i < lastPhases.Length; i++)
                    lastPhases[i] = 0;
            return lastPhases;
        }

        private double[] GenerateSineWave(double frequency, int length, double xInc, double startPhase, out double endPhase)
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

        //public void AddTone(double frequency, double duration)
        //{
        //    long sampleCount = (long)Math.Floor(duration * _sampleRate / 1000d);
        //    _generatedSampleCount += sampleCount;

        //    double amplitude = Math.Pow(2, _bitPerSample - 1) - 1;
        //    double radPerSample = 2 * Math.PI / _sampleRate;
        //    double shift = this.lastSin;
        //    if (this.directionUp)
        //        shift = Math.Asin(shift);
        //    else
        //        shift = -Math.Asin(shift) + Math.PI;
        //    if (_file != null)
        //    {
        //        DataChunk uncompleted = new DataChunk();
        //        for (uint i = 0; i < sampleCount; i++)
        //        {
        //            double sin = Math.Sin(i * radPerSample * frequency + shift);
        //            sin = sin * amplitude;
        //            byte[] sinBytes = ConvertNumber((int)sin, (byte)_bitPerSample);
        //            for (int channel = 0; channel < _channels; channel++)
        //            {
        //                uncompleted.AddSamples(sinBytes);
        //            }
        //        }
        //        Сбрасывание промежуточных результатов
        //        _file.Position = 0;
        //        SaveHeadersToFile(_file);
        //        byte[] uncompleteDataBytes = uncompleted.GetSampleBytes();
        //        uncompleted.ChangeSize(BitConverter.GetBytes((uint)(_bitPerSample / 8 * (_generatedSampleCount * _channels))));
        //        byte[] uncompletedDataHeaderBytes = uncompleted.GetHeaderBytes();
        //        _file.Write(uncompletedDataHeaderBytes, 0, uncompletedDataHeaderBytes.Length);
        //        long headersEnd = _file.Position;
        //        _file.Position += lastDataChunkPosition;
        //        _file.Write(uncompleteDataBytes, 0, uncompleteDataBytes.Length);
        //        lastDataChunkPosition = _file.Position - headersEnd;
        //        padByte
        //        if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
        //        {
        //            _file.Write(new byte[] { 0 }, 0, 1);
        //        }
        //        _file.Flush();
        //    }
        //    else
        //    {
        //        for (uint i = 0; i < sampleCount; i++)
        //        {
        //            double sin = Math.Sin(i * radPerSample * frequency + shift);
        //            sin = sin * amplitude;
        //            byte[] sinBytes = ConvertNumber((int)sin, (byte)_bitPerSample);
        //            for (int channel = 0; channel < _channels; channel++)
        //            {
        //                _data.AddSamples(sinBytes);
        //            }
        //        }
        //    }
        //    this.lastSin = Math.Sin(sampleCount * radPerSample * frequency + shift);
        //    double phase = (duration / 1000 + shift / (2 * Math.PI) / frequency) * frequency;
        //    phase -= (int)(phase);
        //    if (phase >= 0.25 && phase < 0.75)
        //    {
        //        this.directionUp = false;
        //    }
        //    else
        //    {
        //        this.directionUp = true;
        //    }
        //}

        //public void AddTone(double frequency, double duration)
        //{
        //    long sampleCount = (long)Math.Floor(duration * _sampleRate / 1000d);
        //    _generatedSampleCount += sampleCount;

        //    double amplitude = Math.Pow(2, _bitPerSample-1) - 1;
        //    double radPerSample = 2 * Math.PI / _sampleRate;
        //    if (_file != null)
        //    {
        //        DataChunk uncompleted = new DataChunk();
        //        for (uint i = 0; i < sampleCount; i++)
        //        {
        //            double sin = amplitude * Math.Sin(phase);
        //            phase += frequency * radPerSample;
        //            byte[] sinBytes = ConvertNumber(sin, (byte)_bitPerSample);
        //            for (int channel = 0; channel < _channels; channel++)
        //            {
        //                uncompleted.AddSamples(sinBytes);
        //            }
        //        }
        //        //Сбрасывание промежуточных результатов
        //        _file.Position = 0;
        //        SaveHeadersToFile(_file);
        //        byte[] uncompleteDataBytes = uncompleted.GetSampleBytes();
        //        uncompleted.ChangeSize(BitConverter.GetBytes((uint)(_bitPerSample / 8 * (_generatedSampleCount * _channels))));
        //        byte[] uncompletedDataHeaderBytes = uncompleted.GetHeaderBytes();
        //        _file.Write(uncompletedDataHeaderBytes, 0, uncompletedDataHeaderBytes.Length);
        //        long headersEnd = _file.Position;
        //        _file.Position += lastDataChunkPosition;
        //        _file.Write(uncompleteDataBytes, 0, uncompleteDataBytes.Length);
        //        lastDataChunkPosition = _file.Position - headersEnd;
        //        //padByte
        //        if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
        //        {
        //            _file.Write(new byte[] { 0 }, 0, 1);
        //        }
        //        _file.Flush();
        //    }
        //    else
        //    {
        //        for (uint i = 0; i < sampleCount; i++)
        //        {
        //            double sin = amplitude * Math.Sin(phase);
        //            phase += frequency * radPerSample;
        //            byte[] sinBytes = ConvertNumber(sin, (byte)_bitPerSample);
        //            for (int channel = 0; channel < _channels; channel++)
        //            {
        //                data.AddSamples(sinBytes);
        //            }
        //        }
        //    }
        //}

        public byte[] ConvertNumber(long number, byte bit)
        {
            byte[] fullNumber = BitConverter.GetBytes(number);

            byte[] result = new byte[bit / 8];
            //It bit depth is 8
            if (bit == 8)
            {
                sbyte signed = Convert.ToSByte(number);
                byte unsigned = 0;
                unsigned = (byte)(127 + signed);
                result[0] = unsigned;
                return result;
            }
            for (int i = 0; i < bit / 8; i++)
            {
                result[i] = fullNumber[i];
            }
            return result;
        }

        public void SaveTo(Stream stream)
        {
            _header = new HeaderChunk(0);
            _format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
            uint fileSize = (uint)(_header.Size + _format.Size + _data.Size);
            //Check if there's the pad byte
            if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
            {
                fileSize += 1;
            }
            _header.ChangeSize(BitConverter.GetBytes(fileSize));
            byte[] headerbytes = _header.GetChunkBytes();
            byte[] formatBytes = _format.GetChunkBytes();
            byte[] dataBytes = _data.GetChunkBytes();
            stream.Write(headerbytes, 0, headerbytes.Length);
            stream.Write(formatBytes, 0, formatBytes.Length);
            stream.Write(dataBytes, 0, dataBytes.Length);
        }

        private void SaveHeadersToFile(Stream file)
        {
            file.Position = 0;
            _header = new HeaderChunk(0);
            _format = new FormatChunk(_sampleRate, _channels, _bitPerSample);
            uint fileSize = (uint)(_header.Size + _format.Size + 8 + _channels * (_bitPerSample / 8) * _generatedSampleCount);
            //Check if there's the pad byte
            if ((_generatedSampleCount * _channels * (_bitPerSample / 8)) % 2 != 0)
            {
                fileSize += 1;
            }
            _header.ChangeSize(BitConverter.GetBytes(fileSize));
            byte[] headerbytes = _header.GetChunkBytes();
            byte[] formatBytes = _format.GetChunkBytes();
            file.Write(headerbytes, 0, headerbytes.Length);
            file.Write(formatBytes, 0, formatBytes.Length);
        }
    }
}