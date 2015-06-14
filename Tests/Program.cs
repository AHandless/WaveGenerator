using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveGenerator;
using System.Diagnostics;
using System.IO;


namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Tests.Run(Tests.TestLoran, "Loran test");
            Console.ReadKey();
        }
    }
}
