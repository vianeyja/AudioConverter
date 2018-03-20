# Audio Converter for Custom Acoustic Model
This programs provides a few functions that can helps us customize a custom acoustic model.
The audio data recommendations that we cover in this poroject are: [Full Audio Recommendations](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-speech-service/customspeech-how-to-topics/cognitive-services-custom-speech-create-acoustic-model)
1. All audio files in the data set should be stored in the WAV (RIFF) audio format.
2. The audio must have a sampling rate of 8 kHz or 16 kHz and the sample values should be stored as uncompressed PCM 16-bit signed integers (shorts).
3. Only single channel (mono) audio files are supported.
4. The audio files must be between 100ms and 1 minute in length. Each audio file should ideally start and end with at least 100ms of silence, and somewhere between 500ms and 1 second is common.
5. If you have background noise in your data, it is recommended to also have some examples with longer segments of silence, e.g. a few seconds, in your data, before and/or after the speech content.

