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
        //args[0] -g|-s
        //args[1] file path
        //args[2] line length | bucket size
        //args[3] file size
        //args[4] seed
        {
            if (args[0].Equals("-g"))
            {
                var canGenerate = true;

            if (canGenerate)
            {
                canGenerate = Path.IsPathRooted(args[1]);
            }

            if (canGenerate)
            {
                try
                {
                    using (File.Create(args[1])){ }
                    File.Delete(args[1]);
                }
                catch
                {
                    canGenerate = false;
                }
            }

            if (canGenerate)
            {
                try
                {
                    if ( int.Parse(args[2]) < 0 ||
                        long.Parse(args[3]) < 0)
                    {
                        canGenerate = false;
                    }
                    else
                    {
                        if (args.Length > 4)
                        {
                            int.Parse(args[4]);
                        }
                    }
                }
                catch
                {
                    canGenerate = false;
                }
            }

            if (canGenerate == false)
            {
                showError();
            }
            else
            {
                
                var fileFullName = args[1];
                var lineLength = int.Parse(args[2]);
                var fileSize = long.Parse(args[3]);


                var seed = args.Length > 4
                    ? int.Parse(args[4])
                    : new Random().Next(); 
                FileGenerator generator = new TextGenerator(seed, lineLength, fileSize);
                var start = DateTime.Now;
                generator.GenerateFile(fileFullName);
                showDone(DateTime.Now-start);
            }
            }
            else
            {
                if (args[0].Equals("-s"))
                {
                    var canSort = true;
                    if (canSort)
                    {
                        canSort = Path.IsPathRooted(args[1]);
                    }

                    if (canSort)
                    {
                        try
                        {
                            using (File.OpenWrite(args[1])){ }
                        }
                        catch
                        {
                            canSort = false;
                        }
                    }
            
                    if (canSort)
                    {
                        try
                        {
                            canSort = int.Parse(args[2]) > 0;
                        }
                        catch
                        {
                            canSort = false;
                        }
                    }
            
                    if (canSort == false)
                    {
                        showError();
                    }
                    else
                    {
                        var bucketSize = long.Parse(args[2]);
                
                        FileSorter sorter = new MergeSorter(bucketSize);
                        var start = DateTime.Now;
                        sorter.sort(args[1]);
                        showDone(DateTime.Now-start);
                
                    }
                }
            }
            // var now = DateTime.Now;
            // Console.WriteLine("Начинаю профилирование: " + now);
            // var prev = now;
            // FileGenerator generator = new TextGenerator(1,20, 125829120/*1073741824*/);
            // generator.GenerateFile("D:\\test.txt");
            // now = DateTime.Now;
            // Console.WriteLine("Файл сгенерирован за " + (now - prev) + " : " + now);
            // prev = now;
            //
            // MergeSorter sorter = new MergeSorter(10485760/*0*/);
            // sorter.sort("D:\\test.txt");
            // now = DateTime.Now;
            // Console.WriteLine("Файл сгенерирован за " + (now - prev) + " : " + now);
        }

        private static void showError()
        {
            Console.WriteLine("can't do the operation! bye");
        }
        private static void showDone(TimeSpan timeSpan)
        {
            Console.WriteLine("Operation is done! It took: " + timeSpan);
        }
        
    }
}