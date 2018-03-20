using System;
using System.IO;
using System.Collections.Generic;
using NAudio.Wave;

namespace AudioConverter
{
    class Program
    {
        static void Main(string[] args)

        {
            //Read all files in a folder

            string folderPath = @"C:\Users\vianej\Documents\Direct Engagements\Pronto\MP3Audios";
            int i = 0;
            string[] dirs = Directory.GetFiles(folderPath, "*.mp3");

            foreach (string file in dirs)
            {
                
                //Step 1: Convert MP3 to WAV
                string wavOutput = @"C:\Users\vianej\Documents\Direct Engagements\Pronto\WAVAudios\wav" + i + ".wav";
                ConvertMP3toWAV(file, wavOutput);

                //Step 2: Cut audio to 58seconds             
                string wavTrimOutput = @"C:\Users\vianej\Documents\Direct Engagements\Pronto\wavTrim" + i + ".wav";
                TrimWavFile(wavOutput, wavTrimOutput, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 13));

                string noise = @"C:\Users\vianej\Documents\Direct Engagements\Pronto\NoiseAudio\noise.wav";
                string output = @"C:\Users\vianej\Documents\Direct Engagements\Pronto\TrimAudios\audio" + i + ".wav";
                //Step 3: Concatenate Noise
                IEnumerable<string> concatAudio = new string[] { noise, wavTrimOutput, noise };
                Concatenate(output, concatAudio);

                i++;
            }
            
            
        }

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


    }
        
    }

