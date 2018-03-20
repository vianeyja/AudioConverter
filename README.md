# Audio Converter for Custom Acoustic Model
This programs provides a few functions that can helps us customize a custom acoustic model.
The audio data recommendations that we cover in this poroject are: [Full Audio Recommendations](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-speech-service/customspeech-how-to-topics/cognitive-services-custom-speech-create-acoustic-model)
1. All audio files in the data set should be stored in the WAV (RIFF) audio format.
2. The audio must have a sampling rate of 8 kHz or 16 kHz and the sample values should be stored as uncompressed PCM 16-bit signed integers (shorts).
3. Only single channel (mono) audio files are supported.
4. The audio files must be between 100ms and 1 minute in length. Each audio file should ideally start and end with at least 100ms of silence, and somewhere between 500ms and 1 second is common.
5. If you have background noise in your data, it is recommended to also have some examples with longer segments of silence, e.g. a few seconds, in your data, before and/or after the speech content.

The C# project that helps us with this recommendations uses [NAudio](https://github.com/naudio/NAudio), an Open Source .NET library.

The method ConvertMP3toWAV helps us with requirements 1 to 3. Create the wav file monochannel, with 16kHz rate ad a single channel.
The parameters are:
* mp3File: a valid path with our MP3 file (C://Path/mp3File.mp3)
* outputFile: the path where the wav file will be (C://Path/wavFile.wav)

 ```csharp
 public static void ConvertMP3toWAV(string mp3File, string outputFile)
        {
            using (Mp3FileReader reader = new Mp3FileReader(mp3File))
            {

                var newFormat = new WaveFormat(16000, 16, 1);
                using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, conversionStream);
                }

            }

        }
   ```

The methods TrimVavFile helps us with recommendation 4, trimming an Audio with a specific start and end. The parameters are:
* inPath: a valid path with the WAV file we want to trim (C://PahtURL/wavFile.wav).
* outPath: a valid path where the trim wav will be stored (C://PahtURL/wavFileTrim.wav).
* cutFromStart: a TimeSpan where the audio will be start. ```csharp new TimeSpan(0, 0, 0) //Time Span is Hours, Minutes, Seconds ```
* cutFromEnd: a TimeSpan where the audio will end. ```csharp new TimeSpan(0, 1, 0) //Time Span is Hours, Minutes, Seconds ```

    ```csharp
        public static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    int endPos = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endPos = endPos - endPos % reader.WaveFormat.BlockAlign;

                    
                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.WriteData(buffer, 0, bytesRead);
                    }
                }
            }
        }

   ```
 The method Concatenate helps us with recommendation 5, adding sample noise to our audio at the beginning and the end.
 The parameters are: 
 * outputFile: a valid path where the concatenated wav will be stored.
 * sourceFiles: a IEnumerable<string> with the paths of the audios. It is important to put the audios in the following order: 
  1. Audio noise File
  2. The audio file for the model
  3. Audio noise file
 In this way, we'll make sure we'll add noise at the beginning and the end.

  ```csharp
 public static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    using (WaveFileReader reader = new WaveFileReader(sourceFile))
                    {
                        if (waveFileWriter == null)
                        {
                            // first time in create new Writer
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");
                            }
                        }

                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteData(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose();
                }
            }
        } 
   ```
   
  ## References:
  * [Custom Speech Service](https://cris.ai/)

