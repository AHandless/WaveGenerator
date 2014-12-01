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
            SoundGenerator sg = new SoundGenerator(16000, BitDepth.Bit32, 1, file);         
            double[] phase = {0,0,0};
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Random r = new Random();
            for (int i = 0; i < 300; i++)
            {
                phase = sg.AddComplexTone(r.Next(370, 520), phase, 1, true, 400,500,600);
            }
            sg.Save();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();           
        }
    }
}
