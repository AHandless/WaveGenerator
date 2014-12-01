#if DEBUG
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
            WaveFile wavefile = new WaveFile(10000, BitDepth.Bit16, 1, file);          
            SoundGenerator sg = new SoundGenerator(wavefile);
            sg.AddComplexTone(1000, new double[3], 1, false, 400, 500, 600);
            sg.Save(); 
            Console.ReadKey();           
        }
    }
}
#endif
