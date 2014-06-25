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
            StreamWriter log = new StreamWriter("log.txt");
            StreamWriter log2 = new StreamWriter("log2x.txt");
            SoundGenerator sg = new SoundGenerator(16000, 24, 1, null);
         //   Wave w = new Wave();
          //  w.addWave(400, 753);
         
          //  sg.SaveTo(file);
            //sg.AddTone(440, 1097, log2);
            //sg.AddTone(500, 1237, log2);
            //sg.AddTone(600, 1237, log2);
            //sg.AddTone(755, 632, log2);
           //double l = sg.AddToneExperimental(100, 0, 440, 440*2);
           //double l2 = sg.AddToneExperimental(73, l, 440, 440 * 2);
           //sg.AddToneExperimental(800, l2, 440, 440 * 2);
         //   double l3 = sg.AddToneExperimental(753, l2, 261.63, 329.63, 392);
        //  sg.AddToneExperimental(753, l3, 440, 550, 660, 770);
        //   double l2 = sg.AddToneExperimental(1000, l, 440, 510);
         //  sg.AddToneExperimental(440, 1000, l2);

            
        //  sg.AddToneExperimental(753, sg.AddToneExperimental(753, 0, 440), 440);

           Random r = new Random();
          double ppp = 0;        
          for (int i = 0; i < 600; i++)
          {
              int f1 = r.Next(440, 560);
              int f2 = r.Next(560, 620);
              int f3 = r.Next(620, 740);
              ppp = sg.AddSimpleTone(f1, r.Next(70, 120), ppp);
          

          }
       //  sg.AddToneExperimental(1000, ppp, 957, 957, 957);
            sg.SaveTo(file);
            //log.Close();
            //log2.Close();
           // w.saveFile(file2);
           // file2.Close();
            file.Close();
        }
    }
}
