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
* mp3File: a valid URL with our MP3 file (C://PahtURL/mp3File.mp3)
* outputFile: the URL where the wav file will be (C://PahtURL/wavFile.wav)

´´´csharp
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
´´´

