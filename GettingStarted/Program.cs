using System;
using ActLikeAI.Config;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Load("Sample.cfg", @"ActLikeAI/ConfigTest");

            Console.WriteLine($"Size.Width = {Config.Get("Size.Width")}");
            Console.WriteLine($"Size.Height = {Config.Get("Size.Height")}");
            Console.WriteLine();

            Console.WriteLine($"Colors.Foreground:R = {Config.Get("Colors.Foreground:R")}");
            Console.WriteLine($"Colors.Foreground:G = {Config.Get("Colors.Foreground:G")}");
            Console.WriteLine($"Colors.Foreground:B = {Config.Get("Colors.Foreground:B")}");

            Config.Save();
        }
    }
}
