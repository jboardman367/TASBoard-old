# TASBoard
GUI for creating input displays from TAS movie files

## TODO
- Add option to change background colour
- Add context menu to canvas elements
  - change z index
  - delete
- Add option to encode straight onto a video file
  - This requires adding audio processing
  - Option to apply chroma key before putting onto video
- Add more kinds of elements than just keys
  - Waterfalls
  - Static elements
- Add in more ffmpeg options (As a sidebar?)
  - Codec
  - bitrate
- Add more sidebars
  - Properties
  - Encoding Settings (?)
- Add sharing and downloading of style packs
- Add in the rest of the key mappings
- Add in the load style function
- Add save files

## Known Fixes
- Encode button appears to do nothing on Linux
  - This is probably a dependancy issue, try running `sudo apt update && sudo apt install -y libc-dev` and `sudo apt-get install libgdiplus`

## Building From Source
TASBoard used the following NuGet packages:
- SharpZipLib 1.3.3
- FFMediaToolkit 4.1.2
  - For Windows, you will also need to add the libraries listed [here](https://github.com/radek-k/FFMediaToolkit#setup). 
