﻿using System;
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

// Useful links
// https://www.youtube.com/watch?v=fp1Snqq9ovw&t=1053s&ab_channel=G223Productions
// https://stackoverflow.com/questions/19330717/less-than-greater-than-keys-enumeration-in-c-sharp
// http://soundfile.sapp.org/doc/WaveFormat/

/// <summary>
/// From G223 Production's Youtube Video Tutorial
/// https://www.youtube.com/watch?v=fp1Snqq9ovw&t=1053s&ab_channel=G223Productions
/// 
/// Make a design doc first, that way everyone knows what's happening before
/// we're done.
/// 
/// TODO:
///  find out how to use async
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
        private short[] waveData;

        /// <summary>
        /// 
        /// </summary>
        private byte[] binaryWaveData;

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

        /// <summary>
        /// A double rate in Hz (times per second)
        /// </summary>
        public double frequency;

        /// <summary>
        /// Since Z is our lowest key possible, by default,
        /// we want to be able to change what key this is later using this
        /// 
        /// The amount of keys we should shift our alphabet input when converting
        /// to Piano keys and Frequency
        /// 
        /// </summary>
        public int keyOffset = 0;


        /// <summary>
        /// 
        /// </summary>
        public List<AlphabetKeys> keysPressed;

        /// <summary>
        /// 
        /// </summary>
        public List<AlphabetKeys> keysCanceled;



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

            frequency = new MusicNote(SharpNotes.D, 3).GetETFrequency();

        }

        /// <summary>
        /// Called by some process to play exactly one second of a
        /// stream of WAV audio at the desire frequency
        /// </summary>
        private void PlayFrequencyOneSecond(double _frequency) {
            waveData = new short[SAMPLE_RATE];
            binaryWaveData = new byte[SAMPLE_RATE * sizeof(short)];



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

                        WriteSineWave(waveData, _frequency, 1);
                        break;

                    case WaveForm.Square:

                        WriteSquareWave(waveData, _frequency, 1);
                        break;

                    case WaveForm.Saw:

                        WriteSawWave(waveData, _frequency, 1);
                        break;

                    case WaveForm.Triangle:

                        WriteTriangleWave(waveData, _frequency, 1);
                        break;

                    case WaveForm.Noise:

                        WriteNoise(waveData, 1);
                        break;

                }


                // https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c

                // Copy to a byte array for our binary writer
                Buffer.BlockCopy(waveData, 0, binaryWaveData, 0, waveData.Length * sizeof(short));


                // http://soundfile.sapp.org/doc/WaveFormat/
                // Play the generated WAV audio data via a memory stream
                // that we've written onto with a binary writer
                WriteAndPlayWAV(1);

            }
            
        }

        /// <summary>
        /// Called by ___ to play a frequency for a desired length of time
        /// 
        /// TODO:
        /// how do we handle fraction times
        /// </summary>
        /// <param name="_frequency"></param>
        /// <param name="_playTimeInSeconds"></param>
        private void PlayFrequency(double _frequency, short _playTimeInSeconds) {
            random = new Random();
            waveData = new short[SAMPLE_RATE * _playTimeInSeconds];
            binaryWaveData = new byte[SAMPLE_RATE * _playTimeInSeconds * sizeof(short)];

            foreach (Oscillator oscillator in this.Controls.OfType<Oscillator>()) {


                switch (oscillator.Waveform) {

                    case WaveForm.Sine:

                        WriteSineWave(waveData, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Square:

                        WriteSquareWave(waveData, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Saw:

                        WriteSawWave(waveData, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Triangle:

                        WriteTriangleWave(waveData, _frequency, _playTimeInSeconds);
                        break;

                    case WaveForm.Noise:

                        WriteNoise(waveData, _playTimeInSeconds);
                        break;

                }

                // Copy to a byte array for our binary writer
                Buffer.BlockCopy(waveData, 0, binaryWaveData, 0, waveData.Length * sizeof(short));

                // Write our data to a memory stream and play WAV with sound player
                WriteAndPlayWAV(_playTimeInSeconds);

            }

        }

        #region Waves

        public short[] AddWaves(short[] _waveA, short[] _waveB) {
            short[] newWave = new short[SAMPLE_RATE];


            return newWave;

        }


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
                binaryWriter.Write(binaryWaveData);
                memoryStream.Position = 0;

                new SoundPlayer(memoryStream).PlayLooping();
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


        /// <summary>
        /// Convert our keyboard alphabet input
        /// to a Piano Key using our key offset
        /// 
        /// TODO:
        /// Finish this
        /// Test
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public PianoKeys GetPianoKeyFromAlphabetKey(AlphabetKeys alphaKey) {


            return (PianoKeys) ((int) alphaKey + keyOffset);
        }

        /// <summary>
        /// TODO:
        /// Test
        /// 
        /// 
        /// </summary>
        /// <param name="alphaKey"></param>
        /// <returns></returns>
        public double GetFrequencyFromAlphabetKey(AlphabetKeys alphaKey) {

            return (double) GetETFrequencyFromPianoKey( GetPianoKeyFromAlphabetKey(alphaKey));

        }

        #endregion

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


            // For now, play a sample noise when we play any key'
            // 3 seconds long to start
            //PlayFrequency(frequency, 3);



            switch (e.KeyCode) {
                case Keys.Z:
                    // When we press Z, we want to record the key
                    // as a keyboard key (since we can't press it twice 
                    // at any instant)
                    // 0 is Z, 25 is P, for now

                    if(!keysPressed.Contains(AlphabetKeys.Z)) {
                        keysPressed.Add(AlphabetKeys.Z);
                    }

                    break;
                case Keys.X:

                    if (!keysPressed.Contains(AlphabetKeys.X)) {
                        keysPressed.Add(AlphabetKeys.X);
                    }

                    break;
                case Keys.C:

                    if (!keysPressed.Contains(AlphabetKeys.C)) {
                        keysPressed.Add(AlphabetKeys.C);
                    }

                    break;
                case Keys.V:

                    if (!keysPressed.Contains(AlphabetKeys.V)) {
                        keysPressed.Add(AlphabetKeys.V);
                    }

                    break;
                case Keys.B:

                    if (!keysPressed.Contains(AlphabetKeys.B)) {
                        keysPressed.Add(AlphabetKeys.B);
                    }

                    break;
                case Keys.N:

                    if (!keysPressed.Contains(AlphabetKeys.N)) {
                        keysPressed.Add(AlphabetKeys.N);
                    }

                    break;
                case Keys.M:

                    if (!keysPressed.Contains(AlphabetKeys.M)) {
                        keysPressed.Add(AlphabetKeys.M);
                    }

                    break;
                case Keys.A:

                    if (!keysPressed.Contains(AlphabetKeys.A)) {
                        keysPressed.Add(AlphabetKeys.A);
                    }

                    break;
                case Keys.S:

                    if (!keysPressed.Contains(AlphabetKeys.S)) {
                        keysPressed.Add(AlphabetKeys.S);
                    }

                    break;
                case Keys.D:

                    if (!keysPressed.Contains(AlphabetKeys.D)) {
                        keysPressed.Add(AlphabetKeys.D);
                    }

                    break;
                case Keys.F:

                    if (!keysPressed.Contains(AlphabetKeys.F)) {
                        keysPressed.Add(AlphabetKeys.F);
                    }

                    break;
                case Keys.G:

                    if (!keysPressed.Contains(AlphabetKeys.G)) {
                        keysPressed.Add(AlphabetKeys.G);
                    }

                    break;
                case Keys.H:

                    if (!keysPressed.Contains(AlphabetKeys.H)) {
                        keysPressed.Add(AlphabetKeys.H);
                    }

                    break;
                case Keys.J:

                    if (!keysPressed.Contains(AlphabetKeys.J)) {
                        keysPressed.Add(AlphabetKeys.J);
                    }

                    break;
                case Keys.K:

                    if (!keysPressed.Contains(AlphabetKeys.K)) {
                        keysPressed.Add(AlphabetKeys.K);
                    }

                    break;
                case Keys.L:

                    if (!keysPressed.Contains(AlphabetKeys.L)) {
                        keysPressed.Add(AlphabetKeys.L);
                    }

                    break;
                case Keys.Q:

                    if (!keysPressed.Contains(AlphabetKeys.Q)) {
                        keysPressed.Add(AlphabetKeys.Q);
                    }

                    break;
                case Keys.W:

                    if (!keysPressed.Contains(AlphabetKeys.W)) {
                        keysPressed.Add(AlphabetKeys.W);
                    }

                    break;
                case Keys.E:

                    if (!keysPressed.Contains(AlphabetKeys.E)) {
                        keysPressed.Add(AlphabetKeys.E);
                    }

                    break;
                case Keys.R:

                    if (!keysPressed.Contains(AlphabetKeys.R)) {
                        keysPressed.Add(AlphabetKeys.R);
                    }

                    break;
                case Keys.T:

                    if (!keysPressed.Contains(AlphabetKeys.T)) {
                        keysPressed.Add(AlphabetKeys.T);
                    }

                    break;
                case Keys.Y:

                    if (!keysPressed.Contains(AlphabetKeys.Y)) {
                        keysPressed.Add(AlphabetKeys.Y);
                    }

                    break;
                case Keys.U:

                    if (!keysPressed.Contains(AlphabetKeys.U)) {
                        keysPressed.Add(AlphabetKeys.U);
                    }

                    break;
                case Keys.I:

                    if (!keysPressed.Contains(AlphabetKeys.I)) {
                        keysPressed.Add(AlphabetKeys.I);
                    }

                    break;
                case Keys.O:

                    if (!keysPressed.Contains(AlphabetKeys.O)) {
                        keysPressed.Add(AlphabetKeys.O);
                    }
                    break;
                case Keys.P:

                    if (!keysPressed.Contains(AlphabetKeys.P)) {
                        keysPressed.Add(AlphabetKeys.P);
                    }
                    break;
                default:
                    break;
            }


        }

        private void TutorialSynthesizer_KeyUp(object sender, KeyEventArgs e) {

            switch (e.KeyCode) {
                case Keys.Z:
                    
                    if(keysPressed.Contains(AlphabetKeys.Z)) {
                        keysPressed.Remove(AlphabetKeys.Z);
                    }

                    break;
                case Keys.X:

                    if (keysPressed.Contains(AlphabetKeys.X)) {
                        keysPressed.Remove(AlphabetKeys.X);
                    }

                    break;
                case Keys.C:

                    if (keysPressed.Contains(AlphabetKeys.C)) {
                        keysPressed.Remove(AlphabetKeys.C);
                    }

                    break;
                case Keys.V:

                    if (keysPressed.Contains(AlphabetKeys.V)) {
                        keysPressed.Remove(AlphabetKeys.V);
                    }

                    break;
                case Keys.B:

                    if (keysPressed.Contains(AlphabetKeys.B)) {
                        keysPressed.Remove(AlphabetKeys.B);
                    }

                    break;
                case Keys.N:

                    if (keysPressed.Contains(AlphabetKeys.N)) {
                        keysPressed.Remove(AlphabetKeys.N);
                    }
                    break;
                case Keys.M:

                    if (keysPressed.Contains(AlphabetKeys.M)) {
                        keysPressed.Remove(AlphabetKeys.M);
                    }

                    break;
                case Keys.A:

                    if (keysPressed.Contains(AlphabetKeys.A)) {
                        keysPressed.Remove(AlphabetKeys.A);
                    }

                    break;
                case Keys.S:

                    if (keysPressed.Contains(AlphabetKeys.S)) {
                        keysPressed.Remove(AlphabetKeys.S);
                    }

                    break;
                case Keys.D:

                    if (keysPressed.Contains(AlphabetKeys.D)) {
                        keysPressed.Remove(AlphabetKeys.D);
                    }

                    break;
                case Keys.F:

                    if (keysPressed.Contains(AlphabetKeys.F)) {
                        keysPressed.Remove(AlphabetKeys.F);
                    }

                    break;
                case Keys.G:

                    if (keysPressed.Contains(AlphabetKeys.G)) {
                        keysPressed.Remove(AlphabetKeys.G);
                    }

                    break;
                case Keys.H:

                    if (keysPressed.Contains(AlphabetKeys.H)) {
                        keysPressed.Remove(AlphabetKeys.H);
                    }

                    break;
                case Keys.J:

                    if (keysPressed.Contains(AlphabetKeys.J)) {
                        keysPressed.Remove(AlphabetKeys.J);
                    }
                    break;
                case Keys.K:

                    if (keysPressed.Contains(AlphabetKeys.K)) {
                        keysPressed.Remove(AlphabetKeys.K);
                    }

                    break;
                case Keys.L:

                    if (keysPressed.Contains(AlphabetKeys.L)) {
                        keysPressed.Remove(AlphabetKeys.L);
                    }

                    break;
                case Keys.Q:

                    if (keysPressed.Contains(AlphabetKeys.Q)) {
                        keysPressed.Remove(AlphabetKeys.Q);
                    }

                    break;
                case Keys.W:

                    if (keysPressed.Contains(AlphabetKeys.W)) {
                        keysPressed.Remove(AlphabetKeys.W);
                    }

                    break;
                case Keys.E:

                    if (keysPressed.Contains(AlphabetKeys.E)) {
                        keysPressed.Remove(AlphabetKeys.E);
                    }
                    
                    break;
                case Keys.R:

                    if (keysPressed.Contains(AlphabetKeys.R)) {
                        keysPressed.Remove(AlphabetKeys.R);
                    }
                    break;
                case Keys.T:

                    if (keysPressed.Contains(AlphabetKeys.T)) {
                        keysPressed.Remove(AlphabetKeys.T);
                    }
                    break;
                case Keys.Y:
                    if (keysPressed.Contains(AlphabetKeys.Y)) {
                        keysPressed.Remove(AlphabetKeys.Y);
                    }
                    break;
                case Keys.U:

                    if (keysPressed.Contains(AlphabetKeys.U)) {
                        keysPressed.Remove(AlphabetKeys.U);
                    }

                    break;
                case Keys.I:

                    if (keysPressed.Contains(AlphabetKeys.I)) {
                        keysPressed.Remove(AlphabetKeys.I);
                    }

                    break;
                case Keys.O:

                    if (keysPressed.Contains(AlphabetKeys.O)) {
                        keysPressed.Remove(AlphabetKeys.O);
                    }

                    break;
                case Keys.P:

                    if (keysPressed.Contains(AlphabetKeys.P)) {
                        keysPressed.Remove(AlphabetKeys.P);
                    }

                    break;
                default:
                    break;
            }

        }

        private void timer1_Tick(object sender, EventArgs e) {

            //progressBar1.Value = comboBox1.SelectedItem

            if(comboBox1.SelectedItem != null) {
                var device = (MMDevice)comboBox1.SelectedItem;
                progressBar1.Value = (int) Math.Round(device.AudioMeterInformation.MasterPeakValue * 100);
            }


            label1.Text = progressBar1.Value.ToString();

            string myKeysPressed = "[ ";

            for(int a = 0; a < keysPressed.Count; a++) {
                myKeysPressed += keysPressed[a] + " ";
            }

            label2.Text = myKeysPressed + "]";

            // Now for each keyboard key, add waves for each key pressed down to our wave data


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {

            this.ActiveControl = label1;

        }
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
