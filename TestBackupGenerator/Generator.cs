using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;


namespace TestBackupGenerator
{
    public class Generator
    {
        private List<string> paths;

        public void Start()
        {
            paths = new List<string>();
            LoadConfiguration();
            GenerateBackups();
        }

        private void LoadConfiguration(string path = @"config.xml")
        {
            Console.Write("Loading {0}...", path);
            var settings = new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true };

            XmlReader r = XmlReader.Create(path, settings);

            r.ReadToDescendant("Servers");
            r.ReadToDescendant("Server");
            do
            {
                //read all the folder paths out of the configuration xml. we dont need any other data
                r.ReadToDescendant("Folder");
                do
                {
                    paths.Add(r["path"]);
                } while (r.ReadToNextSibling("Folder"));


            } while (r.ReadToNextSibling("Server"));

            r.Close();

            Console.WriteLine("{0} folders found\n", paths.Count);
            
        }

        private void GenerateBackups()
        {
            var rand = new Random();
            foreach (var path in paths)
            {
                int gb = rand.Next(1, 3);
                var writepath = string.Format("{0}\\{1}.tib", path, DateTime.Now.ToString("MMM-dd-yyyy-fffffff"));

                //50% failure rate per folder!
                if (rand.Next(1, 3) > 1)
                {
                    //write a file to disk with a filesize between 1-3gb
                    using (var fs = new FileStream(writepath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        fs.SetLength(1024*1024*1024*gb);
                    }
                    Console.WriteLine("\tWriting {0} ({1}Gb)", writepath, gb);
                }
                else Console.WriteLine("\tSimulating failure for {0}", path);

            }
        }
    }
}
