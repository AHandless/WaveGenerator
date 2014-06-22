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
            SoundGenerator sg = new SoundGenerator(44100, 32, 1, file);
            Random r = new Random();
            for (int i = 0; i < 600; i++)
            {
                int t = r.Next(350, 450);
                sg.AddTone(t, 100);              
            }          
            file.Close();        
        }
    }
}
