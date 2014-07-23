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
         
          
          SoundGenerator sg = new SoundGenerator(22050, BitDepth.Bit32, 1, file);
        
            uint ff = 0;
            sg.AddSimpleTone(500, 30000, 0, 1, 0, out ff, false);
          //sg.Load(file);
          //uint sampleCount = 0;
          //sg.AddSimpleTone(500, 1000, 0, 1, 44100*10, out sampleCount, false);
          //sg.Save();

          //uint sampleCount = 0;
          //uint sampleIndex = 0;
          //double[] startPhase = new double[3];
          //Random r = new Random();
          //Stopwatch sw = new Stopwatch();
          //sw.Start();

          //for (int i = 0; i < 350; i++)
          //{
          //    sampleIndex += sampleCount;
          //    startPhase = sg.AddComplexTone(100, startPhase, 1, sampleIndex, out sampleCount, false, 400, 500, 600);
          //}
          sg.Save();  
        //  Console.ReadKey();
           
        
        }
    }
}
