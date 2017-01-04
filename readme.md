# Wave Generator

Wave Generator is a library for generating sound .wav files.

## Usage Examples
Create an empty .wav file with name `sound.wav` in  the application directory:
```csharp
FileStream file = new FileStream("sound.wav", FileMode.Create);
WaveFile wavefile = new WaveFile(44100, BitDepth.Bit32, 2, file);
wavefile.Save();
```
Generate a sound that has a frequency of 440 Hz and a duration of 100 milliseconds and write it to a file with a name `sound.wav` in the application directory:
```csharp
FileStream file = new FileStream("sound.wav", FileMode.Create);
WaveFile wavefile = new WaveFile(44100, BitDepth.Bit32, 2, file);
SoundGenerator sg = new SoundGenerator(wavefile);
sg.AddSimpleTone(440, 100);
sg.Save();
file.Close();
file.Dispose();
```
You can find more samples in the `Tests` project.