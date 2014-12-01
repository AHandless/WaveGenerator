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
            Stopwatch sw = new Stopwatch();
            FileStream file = new FileStream(@"test.wav", FileMode.Create);
            WaveFile wavefile = new WaveFile(44100, BitDepth.Bit16, 1, file);
            SoundGenerator sg = new SoundGenerator(wavefile);
            sw.Start();
            sg.AddComplexTone(duration: 1000 * 60 * 30,
                              startPhases: new double[3],
                              amplitude: 0.5,
                              fade: true,
                              frequencies: new double[] { 400, 500, 600 });
            //sg.AddSimpleTone(frequency:  400,
            //                 duration:   1000 * 60 * 30,
            //                 startPhase: 0,
            //                 amplitude:  1,
            //                 fade:       false);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sg.Save();
            Console.ReadKey();
        }
    }
}
