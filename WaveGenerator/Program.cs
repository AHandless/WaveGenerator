using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace WaveGenerator
{
    class Program
    {
        static void Main()
        {
            SoundGenerator sg = new SoundGenerator(2000, 16, 1);
            sg.AddTone(400, 1000);
            sg.AddTone(500, 1000);
            sg.AddTone(600, 1000);
            sg.AddTone(700, 1000);
            FileStream file = new FileStream("test.wav", FileMode.Create);
            sg.SaveTo(file);
            file.Close();
        }
    }
}
