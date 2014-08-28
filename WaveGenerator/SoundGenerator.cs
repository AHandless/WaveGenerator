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

        public double AddSimpleTone(double frequency, double duration, double startPhase, double amplitude, bool fade)
        {
            if (duration == 0)
                return 0;
            uint sampleCount = (uint)Math.Floor(duration * _waveFile.SampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double fileAmplitude = Math.Pow(2, (byte)_waveFile.BitDepth - 1) - 1;
            double radPerSample = 2 * Math.PI / _waveFile.SampleRate;

            uint fadeLen = (uint)(sampleCount * 0.10);
            IEnumerator<double> fadeInAmp = Fade(0, amplitude, fadeLen).GetEnumerator();
            IEnumerator<double> fadeOutAmp = Fade(amplitude, 0, fadeLen).GetEnumerator();
            int index = 0;

            foreach (double sample in GenerateSineWave(frequency, sampleCount, radPerSample, startPhase))
            {
                double sin = fileAmplitude * amplitude * sample;
                if (fade && index < fadeLen && fadeInAmp.MoveNext())
                    sin *= fadeInAmp.Current;
                if (fade && index >= sampleCount - fadeLen && fadeOutAmp.MoveNext())
                    sin *= fadeOutAmp.Current;
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_waveFile.BitDepth);

                for (byte channel = 0; channel < _waveFile.Channels; channel++)
                    _waveFile.AddSampleToEnd(sinBytes);

                index++;
            }
            double lastPhase = 0;
            if (!fade)
                lastPhase = GetPhase(radPerSample * sampleCount * frequency + startPhase);
            fadeInAmp.Dispose(); fadeOutAmp.Dispose();
            return lastPhase;
        }

        private IEnumerable<double> Fade(double startAmplitude, double endAmplitude, uint length)
        {
            if (startAmplitude < 0 || endAmplitude < 0)
                throw new ArgumentException("The amplitude can't be negative");
            double minusAmpMax = 0;

            if (startAmplitude > endAmplitude)
            {
                minusAmpMax = startAmplitude - endAmplitude;
                for (int i = 0; i < length; i++)
                    yield return startAmplitude - (minusAmpMax / length * i);
            }
            else
            {
                minusAmpMax = endAmplitude - startAmplitude;
                for (int i = 0; i < length; i++)
                    yield return startAmplitude + (minusAmpMax / length * i);
            }
        }

        public double[] AddComplexTone(double duration, double[] startPhases, double amplitude, bool fade, params double[] frequencies)
        {
            if (duration == 0 ||
                double.IsInfinity(duration) ||
                double.IsNaN(duration) ||
                amplitude < 0 ||
                amplitude > 1 ||
                frequencies == null)
            {
                return startPhases;
            }
            uint sampleCount = (uint)Math.Floor(duration * _waveFile.SampleRate / 1000);
            this._generatedSampleCount += sampleCount;
            double[] lastPhases = new double[frequencies.Length];
            double fileAmplitude = Math.Pow(2, (byte)_waveFile.BitDepth - 1) - 1;
            double radPerSample = 2 * Math.PI / _waveFile.SampleRate;
            double[] complexWave = new double[sampleCount];

            IEnumerator<double>[] waves = new IEnumerator<double>[frequencies.Length];

            for (int frequencyN = 0; frequencyN < frequencies.Length; frequencyN++)
                waves[frequencyN] = GenerateSineWave(frequencies[frequencyN], sampleCount, radPerSample, startPhases[frequencyN]).GetEnumerator();
            
            uint index = 0;
            uint fadeLen = (uint)(sampleCount * 0.10);
            IEnumerator<double> fadeInAmp = Fade(0, amplitude, fadeLen).GetEnumerator();
            IEnumerator<double> fadeOutAmp = Fade(amplitude, 0, fadeLen).GetEnumerator();

            while (waves[0].MoveNext())
            {
                double sin = waves[0].Current;
                for (int i = 1; i < frequencies.Length; i++)
                {
                    waves[i].MoveNext();
                    sin += waves[i].Current;
                }
                sin /= frequencies.Length;
                sin *= fileAmplitude * amplitude;
                if (fade && index < fadeLen && fadeInAmp.MoveNext())
                    sin *= fadeInAmp.Current;
                if (fade && index >= sampleCount - fadeLen && fadeOutAmp.MoveNext())
                    sin *= fadeOutAmp.Current;
                byte[] sinBytes = ConvertNumber((long)sin, (byte)_waveFile.BitDepth);

                for (byte channel = 0; channel < _waveFile.Channels; channel++)
                    _waveFile.AddSampleToEnd(sinBytes);
                index++;
            }
            if (!fade)
                for (int i = 0; i < frequencies.Length; i++)
                {
                    lastPhases[i] = GetPhase(sampleCount * radPerSample * frequencies[i] + startPhases[i]);
                    waves[i].Dispose();
                }
            return lastPhases;
        }

        private IEnumerable<double> GenerateSineWave(double frequency, uint length, double xInc, double startPhase)
        {
            for (int x = 0; x < length; x++)
                yield return Math.Sin(frequency * xInc * x + startPhase);
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

            //It bit depth is 8
            byte[] result = new byte[bit / 8];
            if (bit == 8)
            {
                sbyte signed = Convert.ToSByte(number);
                byte unsigned = 0;
                unsigned = (byte)(128 + signed);
                result[0] = unsigned;
                return result;
            }
            if (bit == 32)
                return BitConverter.GetBytes((int)number);
            if (bit == 16)
                return BitConverter.GetBytes((short)number);
            byte[] fullNumber = BitConverter.GetBytes(number);
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