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
        public delegate TimeSpan TestMethod(SoundGenerator sg);
        private static Random r = new Random();  
        static Tests()
        {
            DirectoryInfo dir = new DirectoryInfo(TestResultDir);
            if (!dir.Exists)
                dir.Create();
            else
                dir.GetFiles().Select(file => { file.Delete(); return file; }).ToArray();
        }

        public static void Run(TestMethod test, string testName)
        {
            FileStream file = new FileStream(
               Path.Combine(TestResultDir, string.Format("{0} {1}.wav", testName, Timestamp)),
               FileMode.Create);
            WaveFile wavefile = new WaveFile(44100, BitDepth.Bit24, 1, file);
            SoundGenerator sg = new SoundGenerator(wavefile);
            TimeSpan time = test(sg);
            sg.Save();
            file.Close();
            file.Dispose();
            Console.WriteLine("Done. {0} Elapsed time: {1}", testName, time.ToString(@"hh\:mm\:ss\.fff"));
        }

        public static TimeSpan TestLoran(SoundGenerator sg)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string sounds = "1180 128;1180 128;1180 128;1180 128;1180 128;1115 128;1115 128;1180 128;1180 128;1115 128;1180 128;1115 128;1180 128";
            string[] separate = sounds.Split(';');
            foreach(string sound in separate)
            {
                string[] note = sound.Split(' ');
                int f = int.Parse(note[0]);
                int d = int.Parse(note[1]);
                sg.AddSimpleTone(f, d, false);
            }
            sw.Stop();
            return sw.Elapsed;
        }

        public static TimeSpan TestRandomChirp(SoundGenerator sg)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double f1 = 0;
            double f2 = 440;
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                f1 = f2;
                f2 = r.Next(300, 4000);
                sg.AddSineChirp(f1, f2, 300);
            }
            sg.AddSimpleTone(f2, 2000, false);
            sw.Stop();
            return sw.Elapsed;
        }

        public static TimeSpan TestChirp(SoundGenerator sg)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sg.AddSimpleTone(440, 2000, false);
            sg.AddSineChirp(440, 900, 3000);
            sg.AddSimpleTone(900, 2000, false);
            sw.Stop();
            return sw.Elapsed;
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
