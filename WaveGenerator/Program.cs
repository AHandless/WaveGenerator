using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaveGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            DataChunk d = new DataChunk();
            d.AddTone(1000, 10000);
            d.SaveTo("test.wav");
        }
    }
}
