using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TASBoard.MovieReaders
{
    public class LibTASReader : IMovieReader
    {
        public MovieSettings MovieSettings { get; private set; }

        private readonly LibTASEnum _enum;
        private readonly string tempDir;

        public void Close()
        {
            // Allow _enum to close
            _enum.Close();

            // Clear out the temp files
            foreach (string file in Directory.EnumerateFiles(tempDir, "*"))
            {
                File.Delete(file);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public LibTASEnum GetEnumerator()
        {
            return _enum;
        }

        public LibTASReader(string fname)
        {
            // Get the temp dir
            tempDir = Path.Join(Path.GetTempPath(), "TASBoard");
            Directory.CreateDirectory(tempDir);

            // Open the file stream
            var movieStream = File.OpenRead(fname);
            var gzipStream = new GZipInputStream(movieStream);

            // Extract the archive into the temp dir
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, null);
            tarArchive.ExtractContents(tempDir);

            // Close the streams
            tarArchive.Close();
            gzipStream.Close();
            movieStream.Close();

            // Initialise variables to put in the movie settings
            int? den = null;
            int? num = null;

            // Open the config file to read values out of
            var configStream = File.OpenRead(Path.Join(tempDir, "config.ini"));
            var configReader = new StreamReader(configStream);

            // Loop over the config and extract the wanted values
            string? line = configReader.ReadLine();
            while (line is not null)
            {
                if (line.StartsWith("framerate_den="))
                {
                    den = int.Parse(line.Substring("framerate_den=".Length));
                }
                else if (line.StartsWith("framerate_num="))
                {
                    num = int.Parse(line.Substring("framerate_num=".Length));
                }

                line = configReader.ReadLine();
            }

            // Close the config file
            configReader.Close();
            configStream.Close();

            // Assign values into the movie settings
            MovieSettings = new MovieSettings(num, den);

            // Create the input enumerator with the input file
            _enum = new LibTASEnum(Path.Join(tempDir, "inputs"));
        }
    }

    public class LibTASEnum : IEnumerator
    {
        readonly Stream inputStream;
        readonly StreamReader inputReader;
        readonly List<string> currentItem = new();
        readonly Dictionary<string, string> keySymToNameDict;

        object IEnumerator.Current { get => Current; }

        public List<string> Current { get => currentItem; }

        public LibTASEnum(string fname)
        {
            // Load the conversion dict
            keySymToNameDict = GetKeySymToNameDict();

            // Open the input files
            inputStream = File.OpenRead(fname);
            inputReader = new(inputStream);


        }

        public void Close()
        {
            inputReader.Close();
            inputStream.Close();
        }

        public bool MoveNext()
        {
            // Read the next line
            string? line = inputReader.ReadLine();

            // If there is no next line, return false
            if (line is null) { return false; }

            // Empty the last frame's inputs
            currentItem.Clear();

            // Iterate over the line to interperet the inputs.
            // See http://tasvideos.org/EmulatorResources/LibTAS/LTMFormat.html
            // for format details.
            foreach (string? section in line.Split('|'))
            {
                // Skip over sections that are empty (There will be at least one created by the ending |)
                if (string.IsNullOrEmpty(section)) { continue; }

                // Handle key sections if any keys are included
                if (section.Length > 1 && section[0] == 'K')
                {
                    // Loop over each key, and add it to the current inputs if it is mapped
                    foreach (string key in section[1..].Split(':'))
                    {
                        if (keySymToNameDict.TryGetValue(key, out string keyName))
                        {
                            currentItem.Add(keyName);
                        }
                    }
                    continue;
                }
                
                // Handle mouse sections
                if (section[0] == 'M')
                {
                    // We will just shortcut to the LMB and RMB here
                    if (section[section.Length - 5] != '.')
                    {
                        currentItem.Add("LMB");
                    }
                    if (section[section.Length - 4] != '.')
                    {
                        currentItem.Add("RMB");
                    }
                }
            }


            // Return true as default
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, string> GetKeySymToNameDict()
        {
            // Create an empty dictionary
            Dictionary<string, string> keySymToName = new() { };

            // Open the mappings file
            var mappingsStream = File.OpenRead("Assets/InputMappings/LibTAS.txt");
            var mappingsReader = new StreamReader(mappingsStream);

            // Read the mappings into the dict
            string? line = mappingsReader.ReadLine();
            while (line is not null)
            {
                line = line.Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    string[] kv = line.Split(',');
                    keySymToName.TryAdd(kv[0], kv[1]);
                }
            }

            // Close the file
            mappingsReader.Close();
            mappingsStream.Close();

            //Return the dict
            return keySymToName;
        }
    }
}
