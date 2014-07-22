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
         //  FileStream file = new FileStream(@"e:\Music\portal2_robots_ftw2.wav", FileMode.OpenOrCreate);
            FileStream file = new FileStream(@"AS.wav", FileMode.OpenOrCreate);
            SoundGenerator sg = new SoundGenerator(8000, BitDepth.Bit8, 2, file);
            sg.Load(file);
            sg.AddSimpleTone(440, 1000, 0, 1, 0,false); 
            //sg.AddSimpleTone(440, 1000, 0, 1, false);
            //sg.AddSimpleTone(440, 1000, 0, 1, false);
            //sg.AddSimpleTone(440, 1000, 0, 1, false);
            sg.Save();
            file.Close();
            //sg.AddSimpleTone(500, 1000, 0, 1, false);
         //   sg.Save();
        
        }
    }
}
