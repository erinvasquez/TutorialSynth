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
        /// <summary>
        /// 
        /// </summary>
        //private short playTimeInSeconds = 2;

        /// <summary>
        /// 
        /// </summary>
        private Random random;

        /// <summary>
        /// 
        /// </summary>
        private short[] wave;

        /// <summary>
        /// 
        /// </summary>
        private byte[] binaryWave;

        /// <summary>
        /// 
        /// </summary>
        private double volumeLevel = 0.22; // otherwise known as Gain
        /// <summary>
        /// 
        /// </summary>
        private double sustainLevel = 0.15; // volume level after a note's "attack"

        /// <summary>
        /// The amount in hz that we offset our frequency to change our note
        /// to a higher or lower one temporarily using some control
        /// </summary>
        private double frequencyOffset = 0.0;

        public double currentFrequency;

        /// <summary>
        /// Since Z is our lowest key possible, by default,
        /// we want to be able to change what key this is later using this
        /// 
        /// 
        /// 
        /// </summary>
        public int keyOffset = 0;




        // Can we have a list of current inputs?
        // like key up and down events so we can add and remove from
        // some strucute/list/stack/etc that will hold the currently
        // playing notes
        public List<AlphabetKeys> keysPressed;



        // private int oscillatorCount = 10 //? We have 10 fingers, should we use this?



        /// <summary>
        /// Initialize our stuff,
        /// Add a device enumerator to detect au
        /// </summary>
        public TutorialSynthesizer() {

            InitializeComponent();

            MMDeviceEnumerator en = new MMDeviceEnumerator();
            var devices = en.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            comboBox1.Items.AddRange(devices.ToArray());

            keysPressed = new List<AlphabetKeys>();

            // Should we make 10 oscillators? Or just play 10 notes
            // on one oscillator (We can add and multiply our wave data to do this)

            currentFrequency = new MusicNote(SharpNotes.D, 3).GetETFrequency();

        }

        /// <summary>
        /// Called by some process to play exactly one second of a
        /// stream of WAV audio at the desire frequency
        /// </summary>
        private void PlayFrequencyOneSecond(double _frequency) {
            wave = new short[SAMPLE_RATE];
            binaryWave = new byte[SAMPLE_RATE * sizeof(short)];



            // 1) Get the frequency we have prepared
            // if it comes from a music note of some sort, convert it to Hz
            // For each oscillator
            // Calculate the wave data that we'll be writing to WAV format

            // Use Buffer.BlockCopy to split our short values into bytes for a byte wave array

            // use a memory stream and binary writer to write our binary wave sample array
            // and output it to a soundplayer


            foreach (Oscillator oscillator in this.Controls.OfType<Oscillator>()) {


                switch (oscillator.Waveform) {

                    case WaveForm.Sine:

                        WriteSineWave(wave, _frequency, 1);
                        break;

                    case WaveForm.Square:

                        WriteSquareWave(wave, _frequency, 1);
                        break;

                    case WaveForm.Saw:

                        WriteSawWave(wave, _frequency, 1);
                        break;

                    case WaveForm.Triangle:

                        WriteTriangleWave(wave, _frequency, 1);
                        break;

                    case WaveForm.Noise:

                        WriteNoise(wave, 1);
                        break;

                }


                // https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c

                // Copy to a byte array for our binary writer
                Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));


                // http://soundfile.sapp.org/doc/WaveFormat/
                // Play the generated WAV audio data via a memory stream
                // that we've written onto with a binary writer
                WriteAndPlayWAV(1);

            }

        }

        /// <summary>
        /// Called by ___ to play a frequency for a desired length o time
        /// </summary>
        /// <param name="_frequency"></param>
        /// <param name="_playTimeInSeconds"></param>
        private void PlayFrequency(double _frequency, short _playTimeInSeconds) {
            random = new Random();
            wave = new short[SAMPLE_RATE * _playTimeInSeconds];
            binaryWave = new byte[SAMPLE_RATE * _playTimeInSeconds * sizeof(short)];

            foreach (Oscillator oscillator in this.Controls.OfType<Oscillator>()) {


                switch (oscillator.Waveform) {

                    case WaveForm.Sine:

                        WriteSineWave(wave, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Square:

                        WriteSquareWave(wave, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Saw:

                        WriteSawWave(wave, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Triangle:

                        WriteTriangleWave(wave, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Noise:

                        WriteNoise(wave, _playTimeInSeconds);
                        break;

                }

                // Copy to a byte array for our binary writer
                Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));

                // Write our data to a memory stream and play WAV with sound player
                WriteAndPlayWAV(_playTimeInSeconds);

            }

        }

        #region Waves

        /// <summary>
        /// Write a sin wave at the desired frequency to our data array
        /// </summary>
        /// <param name="_wave"></param>
        /// <param name="_frequency"></param>
        public void WriteSineWave(short[] _wave, double _frequency, int _playTimeInSeconds) {

            for (int i = 0; i < SAMPLE_RATE; i++) {
                _wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * _frequency) / SAMPLE_RATE * _playTimeInSeconds) * i));
            }

        }

        public void WriteSquareWave(short[] _wave, double _frequency, int _playTimeInSeconds) {

            for (int i = 0; i < SAMPLE_RATE; i++) {
                _wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * _frequency) / SAMPLE_RATE * _playTimeInSeconds) * i));
            }

        }

        public void WriteSawWave(short[] _wave, double _frequency, int _playTimeInSeconds) {
            int samplesPerWaveLength = (int)(SAMPLE_RATE / _frequency);
            short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);
            short tempSample;

            for (int i = 0; i < SAMPLE_RATE * _playTimeInSeconds; i++) {

                tempSample = -short.MaxValue;

                for (int j = 0; j < samplesPerWaveLength && i < SAMPLE_RATE * _playTimeInSeconds; j++) {
                    tempSample += ampStep;
                    _wave[i++] = Convert.ToInt16(tempSample);
                }

                i--;
            }
        }



        public void WriteTriangleWave(short[] _wave, double _frequency, int _playTimeInSeconds) {
            int samplesPerWaveLength = (int)(SAMPLE_RATE / _frequency);
            short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);
            short tempSample;

            tempSample = -short.MaxValue;

            for (int i = 0; i < SAMPLE_RATE * _playTimeInSeconds; i++) {

                if (Math.Abs(tempSample + ampStep) > short.MaxValue) {
                    ampStep = (short)-ampStep;
                    _wave[i++] = Convert.ToInt16(tempSample);
                }

                tempSample += ampStep;
                _wave[i] = Convert.ToInt16(tempSample);
            }

        }

        public void WriteNoise(short[] _wave, int _playTimeInSeconds) {
            random = new Random();

            for (int i = 0; i < SAMPLE_RATE * _playTimeInSeconds; i++) {
                _wave[i] = (short)random.Next(-short.MaxValue, short.MaxValue);
            }
        }

        #endregion

        public void WriteAndPlayWAV(int playTimeInSeconds) {

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


        #region FrequencyOffset
        /// <summary>
        /// 
        /// </summary>
        public void IncrementFrequencyOffset() {
            frequencyOffset += 0.5;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DecrementFrequencyOffset() {
            frequencyOffset -= 0.5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void AddToFrequencyOffset(float offset) {
            frequencyOffset += offset;
        }

        #endregion

        #region getters

        /// <summary>
        /// Get an ETFrequency from a piano key,
        /// A0 to C8
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetETFrequencyFromPianoKey(PianoKeys key) {
            double aForForty = 440.0;

            double a = Math.Pow(2.0, (1.0 / 12.0));

            return (double)aForForty * Math.Pow(a, (double)key);

        }

        #endregion

        #region VolumeControl
        public double VolumeToDb(double volume) {
            return 0.0;
        }
        

        public double DbToVolume(double dB) {
            return 0.0;
        }

        #endregion


        #region Events

        /// <summary>
        /// Is called when we detect a keyboard key go down
        /// 
        /// The only keyboard input we're expecting for now is from our alphabet 
        /// keys playing our synthesizer
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TutorialSynthesizer_KeyDown(object sender, KeyEventArgs e) {

            // Get a frequency from the key we pressed then send it to PlayOneSecondFrequency()


            // For now, play a sample noise when we play any key
            PlayFrequencyOneSecond(currentFrequency);

            switch (e.KeyCode) {
                case Keys.Z:
                    // When we press Z, we want to record the key
                    // as a keyboard key (since we can't press it twice 
                    // at any instant)
                    // 0 is Z, 25 is P, for now
                    keysPressed.Add(AlphabetKeys.Z);
                    
                    break;
                case Keys.X:

                    keysPressed.Add(AlphabetKeys.X);

                    break;
                case Keys.C:
                    break;
                case Keys.V:
                    break;
                case Keys.B:
                    break;
                case Keys.N:
                    break;
                case Keys.M:
                    break;
                case Keys.A:
                    break;
                case Keys.S:
                    break;
                case Keys.D:
                    break;
                case Keys.F:
                    break;
                case Keys.G:
                    break;
                case Keys.H:
                    break;
                case Keys.J:
                    break;
                case Keys.K:
                    break;
                case Keys.L:
                    break;
                case Keys.Q:
                    break;
                case Keys.W:
                    break;
                case Keys.E:
                    break;
                case Keys.R:
                    break;
                case Keys.T:
                    break;
                case Keys.Y:
                    break;
                case Keys.U:
                    break;
                case Keys.I:
                    break;
                case Keys.O:
                    break;
                case Keys.P:
                    break;
                default:
                    break;
            }



            // https://stackoverflow.com/questions/19330717/less-than-greater-than-keys-enumeration-in-c-sharp
            // http://soundfile.sapp.org/doc/WaveFormat/




        }

        private void TutorialSynthesizer_KeyUp(object sender, KeyEventArgs e) {

            switch (e.KeyCode) {
                case Keys.Z:
                    // Assuming nothing weird is going on

                    keysPressed = keysPressed.Where(val => val != (int)PianoKeys.A0).ToArray();

                    break;
                case Keys.X:

                    keysPressed = keysPressed.Where(val => val != (int)PianoKeys.A1).ToArray();

                    break;
                case Keys.C:
                    break;
                case Keys.V:
                    break;
                case Keys.B:
                    break;
                case Keys.N:
                    break;
                case Keys.M:
                    break;
                case Keys.A:
                    break;
                case Keys.S:
                    break;
                case Keys.D:
                    break;
                case Keys.F:
                    break;
                case Keys.G:
                    break;
                case Keys.H:
                    break;
                case Keys.J:
                    break;
                case Keys.K:
                    break;
                case Keys.L:
                    break;
                case Keys.Q:
                    break;
                case Keys.W:
                    break;
                case Keys.E:
                    break;
                case Keys.R:
                    break;
                case Keys.T:
                    break;
                case Keys.Y:
                    break;
                case Keys.U:
                    break;
                case Keys.I:
                    break;
                case Keys.O:
                    break;
                case Keys.P:
                    break;
                default:
                    break;
            }

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

        #endregion

        



    }


    /// <summary>
    /// The currently available waveform types
    /// </summary>
    public enum WaveForm {
        Sine, Square, Saw, Triangle, Noise
    };

    public enum AlphabetKeys {
        Z = 0, X, C, V, B, N, M,
        A, S, D, F, G, H, J, K, L,
        Q, W, E, R, T, Y, U, I, O, P
    };

}
