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
        public MovieProperties MovieProperties { get; private set; }
        public int Length { get => MovieProperties.Length; }

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
            return GetEnumerator1();
        }

        private IEnumerator GetEnumerator1()
        {
            return _enum;
        }

        public IEnumerator<InputFrame> GetEnumerator() { return _enum; }

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
            int den = -1;
            int num = -1;
            int frames = -1;
            bool variableRate = false;

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
                else if (line.StartsWith("frame_count="))
                {
                    frames = int.Parse(line.Substring("frame_count=".Length));
                }
                else if (line.StartsWith("variable_framerate="))
                {
                    variableRate = bool.Parse(line.Substring("variable_framerate=".Length));
                }

                line = configReader.ReadLine();
            }

            // Close the config file
            configReader.Close();
            configStream.Close();

            // Assign values into the movie settings
            MovieProperties = new MovieProperties(
                num, 
                den, 
                frames,
                variableRate);

            // Create the input enumerator with the input file
            _enum = new LibTASEnum(Path.Join(tempDir, "inputs"), MovieProperties);
        }
    }

    public class LibTASEnum : IEnumerator<InputFrame>
    {
        readonly Stream inputStream;
        readonly StreamReader inputReader;
        private InputFrame? currentFrame;
        readonly Dictionary<string, string> keySymToNameDict;
        readonly MovieProperties movieProperties;

        object IEnumerator.Current { get => Current1; }

        private object Current1 { get => Current; }

        public InputFrame Current {
            get
            {
                if (currentFrame == null)
                    throw new InvalidOperationException();

                return (InputFrame)currentFrame;
            }
            
        }

        public LibTASEnum(string fname, MovieProperties properties)
        {
            // Store the properties
            movieProperties = properties;

            // Load the conversion dict
            keySymToNameDict = GetKeySymToNameDict();

            // Open the input files
            inputStream = File.OpenRead(fname);
            inputReader = new(inputStream);


        }

        public void Close()
        {
            if (inputReader != null)
                inputReader.Close();
            if (inputStream != null)
                inputStream.Close();
        }

        public bool MoveNext()
        {
            // Read the next line
            string? line = inputReader.ReadLine();

            // If there is no next line, return false
            if (line is null) { return false; }

            // Initialise some variables to collect frame information
            int framerateNum = movieProperties.FramerateNum;
            int framerateDen = movieProperties.FramerateDen;
            List<string> keys = new();

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
                            keys.Add(keyName);
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
                        keys.Add("LMB");
                    }
                    if (section[section.Length - 4] != '.')
                    {
                        keys.Add("RMB");
                    }
                }

                // Handle variable framerate
                if (section[0] == 'T')
                {
                    string[] rate = section[1..].Split(':');
                    framerateNum = int.Parse(rate[0]);
                    framerateDen = int.Parse(rate[1]);
                }
            }

            currentFrame = new(keys, new Fraction(framerateNum, framerateDen));

            // Return true as default
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // This is where I would dispose of managed resources, if I had any
                }
                currentFrame = null;
                if (inputReader != null)
                {
                    inputReader.Close();
                    inputReader.Dispose();
                }
                if (inputStream != null)
                {
                    inputStream.Close();
                    inputStream.Dispose();
                }                
            }
        }

        ~LibTASEnum()
        {
            Dispose(disposing: false);
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
