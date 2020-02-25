using System;
using System.IO;
using ActLikeAI.Config;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Load("Sample.cfg", Path.Combine("ActLikeAI", "ConfigTest"));

            Console.WriteLine($"Size.Width = {Config.Get("Size.Width")}");
            Console.WriteLine($"Size.Height = {Config.Get("Size.Height")}");
            Console.WriteLine();

            Console.WriteLine($"Colors.Foreground.R = {Config.Get("Colors.Foreground.R")}");
            Console.WriteLine($"Colors.Foreground.G = {Config.Get("Colors.Foreground.G")}");
            Console.WriteLine($"Colors.Foreground.B = {Config.Get("Colors.Foreground.B")}");
            Console.WriteLine();

            Config.Set("Size.Height", "800");
            Config.Set("Colors.Foreground.R", "128");

            Console.WriteLine("After set:");
            Console.WriteLine($"Size.Height = {Config.Get("Size.Height")}");
            Console.WriteLine($"Colors.Foreground.R = {Config.Get("Colors.Foreground.R")}");
            
            Config.Save();
        }
    }
}
