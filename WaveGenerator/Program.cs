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
          //FileStream file = new FileStream(@"e:\Music\portal2_robots_ftw2.wav", FileMode.OpenOrCreate);
          FileInfo file = new FileInfo(@"test.wav");
          SoundGenerator sg = new SoundGenerator(8000, BitDepth.Bit16, 1, file);
          uint sampleCount = 0;
          uint sampleIndex = 0;
          double startPhase = 0;
          Random r = new Random();
          Stopwatch sw = new Stopwatch();
          sw.Start();

          for (int i = 0; i < 350; i++)
          {
              sampleIndex += sampleCount;
              startPhase = sg.AddSimpleTone(500, 100, startPhase, 1, sampleIndex, out sampleCount, false);
          }
          sg.Save();         
          Console.WriteLine(sw.Elapsed);
          Console.ReadKey();
           
        
        }
    }
}
