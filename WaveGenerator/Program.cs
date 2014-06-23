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
            FileStream file = new FileStream("generated.wav", FileMode.Create);
            SoundGenerator sg = new SoundGenerator(8000, 16, 1, file);
            Random r = new Random();
            sg.AddTone(440, 1000);
            sg.AddTone(1000, 1000);
            ////  sg.AddTone(1000, 1000);
            //  //sg.AddTone(400, 433);

            for (int i = 0; i < 573; i++)
            {
                int t = r.Next(350, 450);
                sg.AddTone(t, 73);
            }
            //  sg.SaveTo(file);
            file.Close();
        }
    }
}
