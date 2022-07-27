using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi; // Using NAudio for this one



/// <summary>
/// From G223 Production's Youtube Video Tutorial
/// https://www.youtube.com/watch?v=fp1Snqq9ovw&t=1053s&ab_channel=G223Productions
/// 
/// Make a design doc first, that way everyone knows what's happening before
/// we're done.
/// 
/// 
/// Uses WAV format
/// </summary>
namespace TutorialSynth {

    public partial class TutorialSynthesizer : Form {

        /// <summary>
        /// Typical computer audio sample rate
        /// </summary>
        private const int SAMPLE_RATE = 44_100;

        /// <summary>
        /// In every sample will be 16 bits of binary storage
        /// </summary>
        private const short BITS_PER_SAMPLE = 16;

        // Default code plays enough samples/second to play one second of audio
        // since it literally uses a 44.1 khz (?) sample rate
        private short playTimeInSeconds = 16;

        private Random random;
        private short[] wave;
        private byte[] binaryWave;


        /// <summary>
        /// Initialize our stuff,
        /// Add a device enumerator to detect au
        /// </summary>
        public TutorialSynthesizer() {

            InitializeComponent();

            MMDeviceEnumerator en = new MMDeviceEnumerator();
            var devices = en.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());


        }

        /// <summary>
        /// Called by some process to play exactly one second of a
        /// stream of WAV audio at the desire frequency
        /// </summary>
        private void PlayOneSecondFrequency(float _frequency) {
            random = new Random();
            wave = new short[SAMPLE_RATE * playTimeInSeconds];
            binaryWave = new byte[SAMPLE_RATE * sizeof(short) * playTimeInSeconds];



            // 1) Get the frequency we have prepared
            // if it comes from a music note of some sort, convert it to Hz

            // For each oscillator
            // Calculate the wave data that we'll be writing to WAV format

            // Use Buffer.BlockCopy to split our short values into bytes for a byte wave array

            // use a memory stream and binary writer to write our binary wave sample array
            // and output it to a soundplayer


            foreach (Oscillator oscillator in this.Controls.OfType<Oscillator>()) {
                int samplesPerWaveLength = (int)(SAMPLE_RATE / _frequency);
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);
                short tempSample;


                switch (oscillator.Waveform) {

                    case WaveForm.Sine:

                        for (int i = 0; i < SAMPLE_RATE * playTimeInSeconds; i++) {
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * _frequency) / SAMPLE_RATE) * i));
                        }

                        break;
                    case WaveForm.Square:

                        for (int i = 0; i < SAMPLE_RATE * playTimeInSeconds; i++) {
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin(((Math.PI * 2 * _frequency) / SAMPLE_RATE) * i)));
                        }

                        break;

                    case WaveForm.Saw:

                        for (int i = 0; i < SAMPLE_RATE * playTimeInSeconds; i++) {

                            tempSample = -short.MaxValue;

                            for (int j = 0; j < samplesPerWaveLength && i < SAMPLE_RATE; j++) {
                                tempSample += ampStep;
                                wave[i++] = Convert.ToInt16(tempSample);
                            }

                            i--;
                        }

                        break;


                    case WaveForm.Triangle:

                        tempSample = -short.MaxValue;

                        for (int i = 0; i < SAMPLE_RATE * playTimeInSeconds ; i++) {

                            if (Math.Abs(tempSample + ampStep) > short.MaxValue) {
                                ampStep = (short)-ampStep;
                                wave[i++] = Convert.ToInt16(tempSample);
                            }

                            tempSample += ampStep;
                            wave[i] = Convert.ToInt16(tempSample);
                        }

                        break;

                    case WaveForm.Noise:

                        for (int i = 0; i < SAMPLE_RATE * playTimeInSeconds; i++) {
                            wave[i] = (short)random.Next(-short.MaxValue, short.MaxValue);
                        }


                        break;

                }


                // https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c

            }


            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));


            // http://soundfile.sapp.org/doc/WaveFormat/
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {

                short blockAlign = (short)BITS_PER_SAMPLE / 8; // cast to short "If you perform division on a short it converts to int as you can't perform arithmetic on a short"
                int subChunkTwoSize = SAMPLE_RATE * blockAlign * playTimeInSeconds;

                // ChunkID
                binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });

                // 36 + (8 + SubChunk1Sie) + (8 + Subchunk2Size)
                // This is the size of the rest of the chunk following this number. This
                // is the size of the entire file in bytes minus 8 bytes for the two fields
                // not included in this count:
                // ChunkID and ChunkSize.
                binaryWriter.Write(36 + subChunkTwoSize);
                binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                // "16 for PCM. This is the sieze of the rest of the Subchunk which follows
                // this number"
                binaryWriter.Write(16);
                binaryWriter.Write((short)1);
                // NUmChannels, Mono is 1 Stereo is 2, etc
                binaryWriter.Write((short)1);
                // SampleRate, 8800, 44100, etc.
                binaryWriter.Write(SAMPLE_RATE);
                // ByteRate == SampleRate * NumChannels * BitsPerSample/8
                binaryWriter.Write(SAMPLE_RATE * blockAlign);
                // BlockAlign == NumChannels * BitsPerSample/8
                // The number of bytes for one sample including all channels. Wonder what
                // happens when this number isn't an integer
                binaryWriter.Write(blockAlign);
                // 8 bits = 8, 16 bits = 16, etc
                binaryWriter.Write(BITS_PER_SAMPLE);
                // Subchunk2ID
                binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                // Subchunk2Size == NumSamples * NumChannels * BitsPerSample/8
                binaryWriter.Write(subChunkTwoSize);
                // Data - The actual sound data.
                binaryWriter.Write(binaryWave);
                memoryStream.Position = 0;

                new SoundPlayer(memoryStream).Play();
            }


        }

        /// <summary>
        /// Is called when we detect a keyboard key go down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TutorialSynthesizer_KeyDown(object sender, KeyEventArgs e) {

            // Get a frequency from the key we pressed then send it to PlayOneSecondFrequency()

            PlayOneSecondFrequency(220f);

            // https://stackoverflow.com/questions/19330717/less-than-greater-than-keys-enumeration-in-c-sharp
            // http://soundfile.sapp.org/doc/WaveFormat/

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            // After we change our selection, change focus to another object to avoid typing into it
            // when using the synthesizer
            this.ActiveControl = label1;

        }

        private void progressBar1_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e) {

            if (comboBox1.SelectedItem != null) {
                var singledevice = (MMDevice)comboBox1.SelectedItem;

                // Change 
                progressBar1.Value = (int)(singledevice.AudioMeterInformation.MasterPeakValue * 100);
                label1.Text = progressBar1.Value.ToString();

                // *** SIDE NOTE ***
                // tYPING LIKE THIS AND FIXING IT SOMEHOW. // ==> like this..
                // Tying like CIA AND THEN THIS HAPPENS. // ==> CIA and then this happen
                // everything highlighted has all capatalization reversed 100%

            }

        }
    }

    /// <summary>
    /// The currently available waveform types
    /// </summary>
    public enum WaveForm {
        Sine, Square, Saw, Triangle, Noise
    };

}
