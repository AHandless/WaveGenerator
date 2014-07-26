using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
namespace WaveGenerator
{
    class Program
    {
        static void Main()
        {
            FileStream file = new FileStream(@"test.wav", FileMode.Create);
            SoundGenerator sg = new SoundGenerator(44100, BitDepth.Bit32, 3, file);
            uint sampleCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sg.AddSimpleTone(400, 60 * 1000, 0, 1, 0, out sampleCount, false);
            sg.Save();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();           
        }
    }
}
