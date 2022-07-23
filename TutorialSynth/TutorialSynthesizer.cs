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

        private Random random = new Random();
        private short[] wave = new short[SAMPLE_RATE];
        private byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];
        private float frequency = 220f;

        public TutorialSynthesizer() {

            InitializeComponent();

            MMDeviceEnumerator en = new MMDeviceEnumerator();
            var devices = en.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());

        }

        /// <summary>
        /// Is called when we detect a keyboard key go down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TutorialSynthesizer_KeyDown(object sender, KeyEventArgs e) {

            //MessageBox.Show("Key Down event detected");

            // Generate a second of audio
            // as in generate enough samples of type short, specifically 44,1000 (or whatever it is for your system)
            // 
            /*
            Random random = new Random();
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

            float frequency;
            */
             
            // Decide what frequency to play using our alphabet keys (and a few more) on the keyboard

            // Take advice from this later
            // https://stackoverflow.com/questions/19330717/less-than-greater-than-keys-enumeration-in-c-sharp
            switch (e.KeyCode) {
                case Keys.Z:
                    frequency = 65.41f; // about C2;
                    break;
                case Keys.X:
                    frequency = 69.30f;
                    break;
                case Keys.C:
                    frequency = 73.42f;
                    break;
                case Keys.V:
                    frequency = 77.78f;
                    break;
                case Keys.B:
                    frequency = 82.41f;
                    break;
                case Keys.N:
                    frequency = 87.31f;
                    break;
                case Keys.M:
                    frequency = 92.5f;
                    break;
                case Keys.A:
                    frequency = 98f;
                    break;
                case Keys.S:
                    frequency = 103.83f;
                    break;
                case Keys.D:
                    frequency = 110f;
                    break;
                case Keys.F:
                    frequency = 116.54f;
                    break;
                case Keys.G:
                    frequency = 123.47f;
                    break;
                case Keys.H:
                    frequency = 130.83f;
                    break;
                case Keys.J:
                    frequency = 138.59f;
                    break;
                case Keys.K:
                    frequency = 146.83f;
                    break;
                case Keys.L:
                    frequency = 155.56f;
                    break;
                case Keys.Q:
                    frequency = 164.81f;
                    break;
                case Keys.W:
                    frequency = 174.61f;
                    break;
                case Keys.E:
                    frequency = 185f;
                    break;
                case Keys.R:
                    frequency = 196f;
                    break;
                case Keys.T:
                    frequency = 207.65f;
                    break;
                case Keys.Y:
                    frequency = 220f;
                    break;
                case Keys.U:
                    frequency = 233.08f;
                    break;
                case Keys.I:
                    frequency = 246.94f;
                    break;
                case Keys.O:
                    frequency = 261.63f;
                    break;
                case Keys.P:
                    frequency = 277.18f;
                    break;
                default:
                    // Don't change the frequency just yet?
                    break;
            }

            Console.WriteLine($"Frequency is ", frequency);



            //
            foreach(Oscillator oscillator in this.Controls.OfType<Oscillator>()) {
                int samplesPerWaveLength = (int)(SAMPLE_RATE / frequency);
                short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);
                short tempSample;


                switch(oscillator.Waveform) {

                    case WaveForm.Sine:

                        for (int i = 0; i < SAMPLE_RATE; i++) {
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));
                        }

                        break;
                    case WaveForm.Square:

                        for (int i = 0; i < SAMPLE_RATE; i++) {
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)));
                        }

                        break;

                    case WaveForm.Saw:

                        for (int i = 0; i < SAMPLE_RATE; i++) {

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

                        for(int i = 0; i < SAMPLE_RATE; i++) {

                            if(Math.Abs(tempSample + ampStep) > short.MaxValue) {
                                ampStep = (short)-ampStep;
                                wave[i++] = Convert.ToInt16(tempSample);
                            }
                            
                            tempSample += ampStep;
                            wave[i] = Convert.ToInt16(tempSample);
                        }

                        break;

                    case WaveForm.Noise:

                        for(int i = 0; i < SAMPLE_RATE; i++) {
                            wave[i] = (short)random.Next(-short.MaxValue, short.MaxValue);
                        }


                        break;

                }

                /*
                // Fill these values with data
                for (int i = 0; i < SAMPLE_RATE; i++) {

                    // https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c
                    // Amplitude * sin(angular freq) * t
                    wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));

                }
                */

            }



           

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));

            // http://soundfile.sapp.org/doc/WaveFormat/
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {

                short blockAlign = (short)BITS_PER_SAMPLE / 8; // cast to short "If you perform division on a short it converts to int as you can't perform arithmetic on a short"
                int subChunkTwoSize = SAMPLE_RATE * blockAlign;

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void progressBar1_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e) {

            if(comboBox1.SelectedItem != null) {
                var singledevice = (MMDevice)comboBox1.SelectedItem;
                
                // Change 
                progressBar1.Value = (int)(singledevice.AudioMeterInformation.MasterPeakValue * 100);
                
                Console.WriteLine($"", progressBar1.Value);

                label1.Text = ((int)(singledevice.AudioMeterInformation.MasterPeakValue * 100)).ToString();

                // tYPING LIKE THIS AND FIXING IT SOMEHOW. // ==> like this..
                // Tying like CIA AND THEN THIS HAPPENS. // ==> CIA and then this happen
                // everything highlighted has all capatalization reversed 100%

            }

        }
    }


    public enum WaveForm {
        Sine, Square, Saw, Triangle, Noise
    };

}
