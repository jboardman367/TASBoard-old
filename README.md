# TASBoard
GUI for creating input displays from TAS movie files

## Known Fixes
- Encode button appears to do nothing on Linux
  - This is probably a dependancy issue, try running `sudo apt update && sudo apt install -y libc-dev` and `sudo apt-get install libgdiplus`

## Building From Source
TASBoard used the following NuGet packages:
- SharpZipLib 1.3.3
- FFMediaToolkit 4.1.2
  - For Windows, you will also need to install the libraries listed [here](https://github.com/radek-k/FFMediaToolkit#setup). 
