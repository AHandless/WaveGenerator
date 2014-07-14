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
          
            SoundGenerator sg = new SoundGenerator(7003, BitDepth.Bit32, 1, file);
            Random r = new Random();
            double startPhase = 0;
            for(int i = 0; i<100; i++)
                startPhase = sg.AddSimpleTone(r.Next(400, 500), r.Next(70, 120), startPhase, 1, false);
            sg.Save();
            file.Close();         
        }
    }
}
