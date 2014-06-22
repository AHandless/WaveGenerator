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
            FileStream f = new FileStream("azaza.wav", FileMode.Create);
            StreamWriter logFile = new StreamWriter("log.txt");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SoundGenerator sg = new SoundGenerator(44100, 32, 1, f);
            Random r = new Random();
            //sg.AddTone(400, 100);

            //sg.AddTone(500, 100);
            //sg.AddTone(600, 100);
            //f.Close();
       
           // sg.AddTone(565, 500);
           // sg.AddTone(800, 500);

            for (int i = 0; i < 60; i++)
            {
                int t = r.Next(350, 450);
               
                sg.AddTone(t, 500);
                //sg.AddTone(0, 500);
                logFile.WriteLine("Tone {0} Hz {1} ms at {2}, sample rate {3}", t, 0, sw.Elapsed.ToString(), 44100);
            }

            logFile.WriteLine("TOTAL TIME: {0}, PHASE MAXIMUM: {1}", sw.Elapsed.ToString(), sg.phase);
            f.Close();
            logFile.Close();
           // FileStream file = new FileStream("test.wav", FileMode.Create);
           // sg.SaveTo(file);
          //  file.Close();
           // Console.ReadKey();
        }
    }
}
