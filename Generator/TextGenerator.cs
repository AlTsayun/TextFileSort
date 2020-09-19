using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    public class TextGenerator: FileGenerator
    {
        protected Random random;
        protected int maxLineLength;
        protected long maxFileSize;
        protected int seed { get; }
        
        public TextGenerator(int seed, int maxLineLength, long maxFileSize)
        {
            this.seed = seed;
            this.random = new Random(seed);
            this.maxLineLength = maxLineLength;
            this.maxFileSize = maxFileSize;
        }

        public void GenerateFile(string path)
        {
            bool canCreate;
            try
            {
                using (File.Create(path)) { }
                File.Delete(path);
                canCreate = true;
            }
            catch
            {
                canCreate = false;
            }

            if (canCreate)
            {
                try
                {
                    using(FileStream stream = new FileInfo(path).Create())
                    {
                        var bytesToSave = maxFileSize;
                        while (bytesToSave > 0)
                        {
                            var bytes = Encoding.Unicode.GetBytes(GenerateCharSequence(maxLineLength) + System.Environment.NewLine);
                            bytesToSave -= bytes.Length;
                            stream.Write(bytes, 0, bytes.Length);
                        }

                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                    Console.WriteLine("error while writing");
                }
            }   
        }

        public string GenerateCharSequence (int maxLength)
        { 
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(1, maxLength))
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}