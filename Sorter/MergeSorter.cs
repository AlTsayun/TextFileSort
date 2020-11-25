using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class MergeSorter : FileSorter
    {
        
        private readonly long maxBucketSize;

        public MergeSorter(long maxBucketSize)
        {
            this.maxBucketSize = maxBucketSize;
        }

        public void sort(string fileName)
        {
            var source = new FileInfo(fileName);
            var buckets = separateIntoBuckets(source);
            Parallel.ForEach(buckets, (bucket) => sortBucket(bucket.FullName));
            joinBuckets(buckets, source);
        }

        public List<FileInfo> separateIntoBuckets(FileInfo source)
        {
            
            var buckets = new List<FileInfo>();
            for (var i = 0; i < Math.Ceiling((double) source.Length / maxBucketSize); i++)
            {
                // buckets.Add(new FileInfo(Path.GetTempFileName()));
                buckets.Add(new FileInfo("D:\\" + i + ".txt"));
            }
            
            try
            {
                using(var reader = new StreamReader(source.OpenRead(), Encoding.Unicode))
                {
                    // var enumerator = File.ReadLines(source.FullName).GetEnumerator();

                    for (var i = 0; i < buckets.Count; i++)
                    {
                        try
                        {
                            using(FileStream bucketStream = buckets[i].Create())
                            {
                                var bytesToSave = maxBucketSize;
                                while (bytesToSave > 0)
                                {
                                    if (reader.EndOfStream)
                                    {
                                        break;
                                    }
                                    var bytes = Encoding.Unicode.GetBytes(reader.ReadLine() + System.Environment.NewLine);
                                    // if (!enumerator.MoveNext())
                                    // {
                                    //     break;
                                    // }
                                    // var bytes = Encoding.Unicode.GetBytes(enumerator.Current + System.Environment.NewLine);
                                    bytesToSave -= bytes.Length;
                                    bucketStream.Write(bytes, 0, bytes.Length);
                                }

                                bucketStream.Close();
                            }
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("error while writing to bucket: " + buckets[i].FullName);
                        }
                            
                    }
                    reader.Close();
                    
                }
            }
            catch (IOException)
            {
                Console.WriteLine("error while reading from file: " + source.FullName);
            }

            return buckets;
        }

        public void sortBucket(string fileName)
        {
            try
            {

                var lines = File.ReadAllLines(fileName, Encoding.Unicode);

                Array.Sort(lines);
                // int size = lines.Length;
                //
                // for (int i = size / 2 - 1; i >= 0; i--)
                //     heapify(lines, size, i);
                //
                // for (int i=size-1; i>=0; i--)
                // {
                //     string temp = lines[0];
                //     lines[0] = lines[i];
                //     lines[i] = temp;
                //
                //     heapify(lines, i, 0);
                // }

                File.WriteAllLines(fileName, lines, Encoding.Unicode);
            }
            catch (IOException)
            {
                Console.WriteLine("error while io from file: " + fileName);
            }

        }

        private void heapify(string[] lines, int size, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if (left < size && lines[left].CompareTo(lines[largest]) > 0)
                largest = left;

            if (right < size && lines[right].CompareTo(lines[largest]) > 0)
                largest = right;
            
            if (largest != i)
            {
                string swap = lines[i];
                lines[i] = lines[largest];
                lines[largest] = swap;

                heapify(lines, size, largest);
            }
            
        }
        


        public void joinBuckets(List<FileInfo> source, FileInfo dest)
        {
            var bucketCount = source.Count;
            for (var i = 1; i < bucketCount; i++)
            {
                var tmpFile = new FileInfo(Path.GetTempFileName());
                using (var leftReader = new StreamReader(source[0].OpenRead(), Encoding.Unicode))
                using (var rightReader = new StreamReader(source[1].OpenRead(), Encoding.Unicode))
                using (var tmpWriter = new StreamWriter(tmpFile.OpenWrite(), Encoding.Unicode))
                {
                    string leftLine = null;
                    string rightLine = null;
                    while (!(leftReader.EndOfStream || rightReader.EndOfStream))
                    {
                        if (leftLine == null)
                        {
                            leftLine = leftReader.ReadLine();
                        }

                        if (rightLine == null)
                        {
                            rightLine = rightReader.ReadLine();
                        }

                        if (leftLine.CompareTo(rightLine) < 0)
                        {
                            tmpWriter.WriteLine(leftLine);
                            leftLine = null;
                        }
                        else
                        {
                            tmpWriter.WriteLine(rightLine);
                            rightLine = null;
                        }
                    }

                    if (!rightReader.EndOfStream)
                    {
                        var remaining = rightReader.ReadToEnd();
                        tmpWriter.Write(remaining);
                    }

                    if (!leftReader.EndOfStream)
                    {
                        var remaining = leftReader.ReadToEnd();
                        tmpWriter.Write(remaining);
                    }


                }

                source[1].Delete();
                source[0].Delete();
                source.RemoveAt(1);
                source[0] = tmpFile;
            }
            var fileName = dest.FullName;
            dest.Delete();
            File.Move(source[0].FullName, fileName);
        }
    }

}