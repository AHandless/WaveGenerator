using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    public class SoundGenerator
    {
        private uint _generatedSampleCount = 0;
        private WaveFile _waveFile;
        
        public Stream File
        {
            get
            {
                return _waveFile.File;
            }
        }      

        public SoundGenerator(uint sampleRate, BitDepth bitDepth, ushort channels, Stream file)
        {
            if (Enum.IsDefined(typeof(BitDepth), bitDepth) == false)
                throw new ArgumentException("Unsupported bit depth", "bitDepth");
            _waveFile = new WaveFile(sampleRate, bitDepth, channels, file);                  
        }
        public SoundGenerator()
        {            
            _waveFile = new WaveFile();
        }

        public double AddSimpleTone(double frequency, 
                                    double duration, 
                                    double startPhase, 
                                    double amplitude, 
                                    uint sampleIndex, 
                                    out uint sampleCount, 
                                    bool fade)
        {
            if (duration == 0)
            {
                sampleCount = 0;
                return 0;
            }
            double lastPhase = 0;
            sampleCount = (uint)Math.Floor(duration * _waveFile.SampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double fileAmplitude = Math.Pow(2, (byte)_waveFile.BitDepth - 1) - 1;
            double radPerSample = 2 * Math.PI / _waveFile.SampleRate;

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
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_waveFile.BitDepth);

                for (byte channel = 0; channel < _waveFile.Channels; channel++)
                {
                    _waveFile.AddSample(sinBytes, sampleIndex + i, channel);
                }
            }
            if (fade)
                lastPhase = 0;
            return lastPhase;
        }

        private void Fade(ref double[] wave, double startAmplitude, double endAmplitude, uint start, uint length)
        {
            if (start >= wave.Length || length > wave.Length - start)
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

        public double[] AddComplexTone(double duration,
                                       double[] startPhases,
                                       double amplitude,
                                       uint sampleIndex,
                                       out uint sampleCount,
                                       bool fade,
                                       params double[] frequencies)
        {
            if (duration == 0 ||
                double.IsInfinity(duration) ||
                double.IsNaN(duration) ||
                amplitude < 0 ||
                amplitude > 1 ||
                frequencies == null)
            {
                sampleCount = 0;
                return startPhases;
            }
            sampleCount = (uint)Math.Floor(duration * _waveFile.SampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double[] lastPhases = new double[frequencies.Length];
            double fileAmplitude = Math.Pow(2, (byte)_waveFile.BitDepth - 1) - 1;
            double radPerSample = 2 * Math.PI / _waveFile.SampleRate;
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
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_waveFile.BitDepth);
                for (byte channel = 0; channel < _waveFile.Channels; channel++)
                {
                    _waveFile.AddSample(sinBytes, sampleIndex + i, channel);
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
            _waveFile.Save();                   
        }

        public void Load(string filePath)
        {
            _waveFile.Load(filePath);
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