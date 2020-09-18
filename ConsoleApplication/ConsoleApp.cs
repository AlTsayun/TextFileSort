using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace ConsoleApplication
{
    internal class ConsoleApp
    {
        public static void Main(string[] args)
        {
            var now = DateTime.Now;
            Console.WriteLine("Начинаю профилирование: " + now);
            var prev = now;
            FileGenerator generator = new TextGenerator(1,20, 125829120/*1073741824*/);
            generator.GenerateFile("D:\\test.txt");
            now = DateTime.Now;
            Console.WriteLine("Файл сгенерирован за " + (now - prev) + " : " + now);
            prev = now;
            
            MergeSorter sorter = new MergeSorter(10485760/*0*/);
            sorter.sort("D:\\test.txt");
            now = DateTime.Now;
            Console.WriteLine("Файл сгенерирован за " + (now - prev) + " : " + now);
        }

    }
}