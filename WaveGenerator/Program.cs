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
          
            SoundGenerator sg = new SoundGenerator(7003, BitDepth.Bit8, 1, file);
            Random r = new Random();
            double startPhase = 0;        
            sg.AddSimpleTone(440, 1000, 0, 1, false);            
            sg.Save();
            file.Close();         
        }
    }
}
