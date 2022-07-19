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



        public TutorialSynthesizer() {

            InitializeComponent();

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
            short[] wave = new short[SAMPLE_RATE];

            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];

            // A4
            float frequency = 440f;

            // Fill these values with data
            for (int i = 0; i < SAMPLE_RATE; i++) {

                // https://docs.microsoft.com/en-us/archive/blogs/dawate/intro-to-audio-programming-part-3-synthesizing-simple-wave-audio-using-c
                // Amplitude * sin(angular freq) * t
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin( ((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));

            }

            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));

            // http://soundfile.sapp.org/doc/WaveFormat/
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {

                short blockAlign = (short)BITS_PER_SAMPLE / 8; // cast to short "If you perform division on a short it converts to int as you can't perform arithmetic on a short"
                int subChunkTwoSize = SAMPLE_RATE * blockAlign;

                binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                binaryWriter.Write(36 + subChunkTwoSize);
                binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                binaryWriter.Write(16);
                binaryWriter.Write((short)1);
                binaryWriter.Write((short)1);
                binaryWriter.Write(SAMPLE_RATE);
                binaryWriter.Write(SAMPLE_RATE * blockAlign);
                binaryWriter.Write(blockAlign);
                binaryWriter.Write(BITS_PER_SAMPLE);
                binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                binaryWriter.Write(subChunkTwoSize);
                binaryWriter.Write(binaryWave);
                memoryStream.Position = 0;

                new SoundPlayer(memoryStream).Play();
            }




        }

    }

}
