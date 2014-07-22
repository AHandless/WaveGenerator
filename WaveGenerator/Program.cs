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

            FileStream fi = new FileStream(@"c:\Users\Alexander\Desktop\О. С. П. — Какао_d.wav", FileMode.OpenOrCreate);
            SoundGenerator sg = new SoundGenerator();
            sg.Load(fi);
            sg.AddSimpleTone(800, 3000, 0, 1, false);
            sg.Save();
        }
    }
}
