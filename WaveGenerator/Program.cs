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
            FileStream file = new FileStream("AS.wav", FileMode.Create);
          
            SoundGenerator sg = new SoundGenerator(44100, BitDepth.Bit32, 1, file);
            Random r = new Random();
            double startPhase = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sg.AddSimpleTone(440, 2000, 0, 0.3, true);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();            
            sg.Save();
            file.Close();         
        }
    }
}
