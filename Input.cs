﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FileIO
{
    public class Input
    {
        private string InputFolder { get; set; }
        private string[] InputFiles { get; set; }
        public string[] UitgepaktZipFolders { get; set; }
        public Input()
        {
            //Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Input"));
            InputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Input");
            InputFiles = Directory.GetFiles(InputFolder);
            ExtractZipFiles();
            UitgepaktZipFolders = Directory.GetDirectories(InputFolder);
        }
        private void ExtractZipFiles()
        {
            if (InputFiles.Length != 0)
            {
                foreach (string d in InputFiles)
                {
                    if (d.Contains(".zip"))
                    {
                        if (!Directory.Exists(Path.Combine(InputFolder, Path.GetFileNameWithoutExtension(d))))
                            ZipFile.ExtractToDirectory(d, InputFolder);
                    }
                }
            }
        }
    }
}
