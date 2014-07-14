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
          
            SoundGenerator sg = new SoundGenerator(44100, BitDepth.Bit32, 1, null);
           
            sg.SaveTo(file);
            file.Close();         
        }
    }
}
