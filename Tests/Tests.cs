using System;
using WaveGenerator;
using System.Diagnostics;
using System.IO;
using System.Linq;
namespace Tests
{
    static class Tests
    {
        public static string Timestamp
        {
            get
            {
                return DateTime.Now.ToString(@"on dd.MM.yyyy a\t HH-mm-ss"); 
            }
            
        }
        public static string TestResultDir { get { return @".\TestResults"; } }
        private static Random r = new Random();  
        static Tests()
        {
            DirectoryInfo dir = new DirectoryInfo(TestResultDir);
            if (!dir.Exists)
                dir.Create();
            else
                dir.GetFiles().Select(file => { file.Delete(); return file; }).ToArray();
        }

        public static void TestSimple()
        {         
            FileStream file = new FileStream(
                Path.Combine(TestResultDir, string.Format("{0} {1}.wav", "Simple tone generation test", Timestamp)),
                FileMode.Create);
            WaveFile wavefile = new WaveFile(44100, BitDepth.Bit32, 2, file);
            SoundGenerator sg = new SoundGenerator(wavefile);
            sg.AddSimpleTone(440, 1000);
            sg.Save();
            file.Close();
            file.Dispose();
            Console.WriteLine("Simple tone generation test.");
        }

        public static void TestClicks()
        {            
            FileStream file = new FileStream(
                Path.Combine(TestResultDir, string.Format("{0} {1}.wav", "Click test", Timestamp)),
                FileMode.Create);
            WaveFile wavefile = new WaveFile(44100, BitDepth.Bit32, 2, file);
            SoundGenerator sg = new SoundGenerator(wavefile);
            for(int i = 0; i< 100; i++)           
                sg.AddSimpleTone(300, r.Next(100, 600), false);
            sg.Save();
            file.Close();
            file.Dispose();
            Console.WriteLine("Click test.");
         
        }

        public static void TestComplex()
        {            
            FileStream file = new FileStream(
                Path.Combine(TestResultDir, string.Format("{0} {1}.wav", "Complex tone generation test", Timestamp)),
                FileMode.Create);
            WaveFile wavefile = new WaveFile(44100, BitDepth.Bit32, 2, file);
            SoundGenerator sg = new SoundGenerator(wavefile);
            for (int i = 0; i < 100; i++)
                sg.AddComplexTone(r.Next(100,500), false, 261.63, 329.63, 392);              
            sg.Save();
            file.Close();
            file.Dispose();
            Console.WriteLine("Complex tone generation test.");
        }
    }
}
